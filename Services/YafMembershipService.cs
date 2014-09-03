using Globalcaching.Core;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Messaging.Services;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Globalcaching.Services
{
    [OrchardSuppressDependency("Orchard.Users.Services.MembershipService")]
    public class YafMembershipService : IMembershipService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMessageService _messageService;
        private readonly IEnumerable<IUserEventHandler> _userEventHandlers;
        private readonly IEncryptionService _encryptionService;
        private readonly IShapeFactory _shapeFactory;
        private readonly IShapeDisplay _shapeDisplay;

        public YafMembershipService(
            IOrchardServices orchardServices,
            IMessageService messageService,
            IEnumerable<IUserEventHandler> userEventHandlers,
            IClock clock,
            IEncryptionService encryptionService,
            IShapeFactory shapeFactory,
            IShapeDisplay shapeDisplay)
        {
            _orchardServices = orchardServices;
            _messageService = messageService;
            _userEventHandlers = userEventHandlers;
            _encryptionService = encryptionService;
            _shapeFactory = shapeFactory;
            _shapeDisplay = shapeDisplay;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public MembershipSettings GetSettings()
        {
            var settings = new MembershipSettings();
            // accepting defaults
            return settings;
        }

        public IUser CreateUser(CreateUserParams createUserParams)
        {
            Logger.Information("CreateUser {0} {1}", createUserParams.Username, createUserParams.Email);

            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();

            var user = _orchardServices.ContentManager.New<UserPart>("User");

            user.UserName = createUserParams.Username;
            user.Email = createUserParams.Email;
            user.NormalizedUserName = createUserParams.Username.ToLowerInvariant();
            user.HashAlgorithm = "SHA1";
            SetPassword(user, createUserParams.Password);

            if (registrationSettings != null)
            {
                user.RegistrationStatus = registrationSettings.UsersAreModerated ? UserStatus.Pending : UserStatus.Approved;
                user.EmailStatus = registrationSettings.UsersMustValidateEmail ? UserStatus.Pending : UserStatus.Approved;
            }

            if (createUserParams.IsApproved)
            {
                user.RegistrationStatus = UserStatus.Approved;
                user.EmailStatus = UserStatus.Approved;
            }

            var userContext = new UserContext { User = user, Cancel = false, UserParameters = createUserParams };
            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.Creating(userContext);
            }

            if (userContext.Cancel)
            {
                return null;
            }

            _orchardServices.ContentManager.Create(user);

            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.Created(userContext);
                if (user.RegistrationStatus == UserStatus.Approved)
                {
                    userEventHandler.Approved(user);
                }
            }

            if (registrationSettings != null
                && registrationSettings.UsersAreModerated
                && registrationSettings.NotifyModeration
                && !createUserParams.IsApproved)
            {
                var usernames = String.IsNullOrWhiteSpace(registrationSettings.NotificationsRecipients)
                                    ? new string[0]
                                    : registrationSettings.NotificationsRecipients.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var userName in usernames)
                {
                    if (String.IsNullOrWhiteSpace(userName))
                    {
                        continue;
                    }
                    var recipient = GetUser(userName);
                    if (recipient != null)
                    {
                        var template = _shapeFactory.Create("Template_User_Moderated", Arguments.From(createUserParams));
                        template.Metadata.Wrappers.Add("Template_User_Wrapper");

                        var parameters = new Dictionary<string, object> {
                            {"Subject", T("New account").Text},
                            {"Body", _shapeDisplay.Display(template)},
                            {"Recipients", new [] { recipient.Email }}
                        };

                        _messageService.Send("Email", parameters);
                    }
                }
            }

            return user;
        }

        public IUser GetUser(string username)
        {
            var lowerName = username == null ? "" : username.ToLowerInvariant();

            return _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.NormalizedUserName == lowerName).List().FirstOrDefault();
        }

        public IUser ValidateUser(string userNameOrEmail, string password)
        {
            var lowerName = userNameOrEmail == null ? "" : userNameOrEmail.ToLowerInvariant();

            var user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.NormalizedUserName == lowerName).List().FirstOrDefault();

            if (user == null)
                user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.Email == lowerName).List().FirstOrDefault();

            if (user == null)
            {
                //check if user exists in YAF
                //if so, create user
                try
                {
                    using (SqlConnection con = new SqlConnection(DBCon.dbForumConnString))
                    {
                        con.Open();
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = string.Format("select Name, EMail from yaf_User where Name='{0}'", lowerName.Replace("'", "''"));
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.Read())
                            {
                                CreateUserParams createUserParams = new CreateUserParams(dr.GetString(0), "234834284gf", dr.GetString(1), "No question", "No answer for this one", true);
                                IUser cu = CreateUser(createUserParams);
                                user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.NormalizedUserName == lowerName).List().FirstOrDefault();
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            if (user == null || ValidatePassword(user.As<UserPart>(), password) == false)
                return null;

            if (user.EmailStatus != UserStatus.Approved)
                return null;

            if (user.RegistrationStatus != UserStatus.Approved)
                return null;

            return user;
        }

        public void SetPassword(IUser user, string password)
        {
            if (!user.Is<UserPart>())
                throw new InvalidCastException();

            var userPart = user.As<UserPart>();

            if (userPart.UserName.ToLower() == "host")
            {
                SetPasswordHashed(userPart, password);
            }
            else
            {
                userPart.PasswordFormat = MembershipPasswordFormat.Clear;
                userPart.Password = password;
                userPart.PasswordSalt = null;
            }
        }

        private bool ValidatePassword(UserPart userPart, string password)
        {
            bool valid = false;
            if (userPart.UserName.ToLower() == "host")
            {
                valid = ValidatePasswordHashed(userPart, password);
            }
            else
            {
                try
                {
                    System.Net.HttpWebRequest webRequest = System.Net.WebRequest.Create(string.Format("http://www.globalcaching.eu/Layar/ccc.aspx?usr={0}&pwd={1}", HttpUtility.UrlEncode(userPart.UserName), HttpUtility.UrlEncode(password))) as System.Net.HttpWebRequest;
                    webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.17 Safari/533.4";
                    using (System.IO.StreamReader responseReader = new System.IO.StreamReader(webRequest.GetResponse().GetResponseStream()))
                    {
                        // and read the response
                        string doc = responseReader.ReadToEnd();
                        valid = doc == "OK";
                    }
                }
                catch
                {
                }
            }
            return valid;
        }

        private static void SetPasswordHashed(UserPart userPart, string password)
        {

            var saltBytes = new byte[0x10];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes);
            }

            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

            byte[] hashBytes;
            using (var hashAlgorithm = HashAlgorithm.Create(userPart.HashAlgorithm))
            {
                hashBytes = hashAlgorithm.ComputeHash(combinedBytes);
            }

            userPart.PasswordFormat = MembershipPasswordFormat.Hashed;
            userPart.Password = Convert.ToBase64String(hashBytes);
            userPart.PasswordSalt = Convert.ToBase64String(saltBytes);
        }

        private static bool ValidatePasswordHashed(UserPart userPart, string password)
        {

            var saltBytes = Convert.FromBase64String(userPart.PasswordSalt);

            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

            byte[] hashBytes;
            using (var hashAlgorithm = HashAlgorithm.Create(userPart.HashAlgorithm))
            {
                hashBytes = hashAlgorithm.ComputeHash(combinedBytes);
            }

            return userPart.Password == Convert.ToBase64String(hashBytes);
        }


    }
}
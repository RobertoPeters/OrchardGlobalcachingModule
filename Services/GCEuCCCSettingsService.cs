using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Globalcaching.Services
{
    public interface IGCEuCCCSettingsService : IDependency
    {
        GCEuCCCUser GetSettings();
        GCEuCCCUser GetSettings(string username);
        void UpdateSettings(GCEuCCCUser settings);
        CheckCCCResult GetCCCUsersForGeocache(int page, int pageSize, string GCCode, bool removeEmailAddress);
        string GetCCCServiceResult(string gccode, string username, string password);
        List<ListCCCMembersModel> GetAllCCCUsers();
        void DeactivateCCCMember(int id);
        GCEuCCCRequestModel GetRequestCalls(int page, int pageSize);
    }

    public class GCEuCCCSettingsService : IGCEuCCCSettingsService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IOrchardServices _orchardServices;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IMembershipService _membershipService;

        public GCEuCCCSettingsService(IWorkContextAccessor workContextAccessor,
            IOrchardServices orchardServices,
            IGCEuUserSettingsService gcEuUserSettingsService,
            IMembershipService membershipService)
        {
            _workContextAccessor = workContextAccessor;
            _orchardServices = orchardServices;
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _membershipService = membershipService;
        }

        public void DeactivateCCCMember(int id)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                db.Execute("update GCEuCCCUser set Active=0 where UserID=@0", id);
            }
        }

        public List<ListCCCMembersModel> GetAllCCCUsers()
        {
            List<ListCCCMembersModel> result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<ListCCCMembersModel>("select GCEuCCCUser.*, yaf_user.Name, GCComUser.UserName from GCEuCCCUser inner join Globalcaching.dbo.yaf_user on GCEuCCCUser.UserID = yaf_user.UserID inner join GCComData.dbo.GCComUser on GCEuCCCUser.GCComUserID = GCComUser.ID order by ModifiedAt desc");
            }
            return result;
        }

        public GCEuCCCUser GetSettings()
        {
            GCEuCCCUser result = null;
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    result = db.FirstOrDefault<GCEuCCCUser>("where UserID = @0", settings.YafUserID);
                }
            }
            return result;
        }

        public GCEuCCCUser GetSettings(string username)
        {
            GCEuCCCUser result = null;
            var settings = _gcEuUserSettingsService.GetSettings(username);
            if (settings != null && settings.YafUserID > 1)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    result = db.FirstOrDefault<GCEuCCCUser>("where UserID = @0", settings.YafUserID);
                }
            }
            return result;
        }

        public void UpdateSettings(GCEuCCCUser settings)
        {
            if (settings != null && settings.UserID > 1)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    if (db.FirstOrDefault<GCEuCCCUser>("where UserID = @0", settings.UserID) != null)
                    {
                        db.Update("GCEuCCCUser", "UserID", settings);
                    }
                    else
                    {
                        db.Insert(settings);
                    }
                }
            }            
        }

        public class GeocacheInfo
        {
            public long ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public long? OwnerId { get; set; }
        }

        public CheckCCCResult GetCCCUsersForGeocache(int page, int pageSize, string GCCode, bool removeEmailAddress)
        {
            CheckCCCResult result = new CheckCCCResult();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                string gcComDatabase = Core.Helper.GetTableNameFromConnectionString(dbGcComDataConnString);
                string yafDatabase = Core.Helper.GetTableNameFromConnectionString(Core.DBCon.dbForumConnString);

                var gcInfo = db.FirstOrDefault<GeocacheInfo>(string.Format("select ID, Code, Name, OwnerId from [{0}].[dbo].[GCComGeocache] where Code=@0", gcComDatabase), GCCode);
                if (gcInfo != null)
                {
                    result.GeocacheCode = gcInfo.Code;
                    result.GeocacheTitle = gcInfo.Name;

                    var sql2 = PetaPoco.Sql.Builder.Append("select GCEuCCCUser.*, yaf_User.EMail, GCComUser.UserName, GCComUser.AvatarUrl, GCComUser.PublicGuid, GCComUser.FindCount, GETDATE() as VisitDate, '' as Url")
                        .From("GCEuCCCUser with (nolock)")
                        .InnerJoin(string.Format("[{0}].[dbo].[GCComUser] with (nolock)", gcComDatabase)).On("GCEuCCCUser.GCComUserID=GCComUser.ID")
                        .InnerJoin(string.Format("[{0}].[dbo].[yaf_User] with (nolock)", yafDatabase)).On("GCEuCCCUser.UserID=yaf_User.UserID")
                        .Where("GCEuCCCUser.GCComUserID=@0 and GCEuCCCUser.Active=1", gcInfo.OwnerId);
                    
                    result.Owner = db.FirstOrDefault<CheckCCCItem>(sql2);

                    var sql = PetaPoco.Sql.Builder.Append("select GCEuCCCUser.*, yaf_User.EMail, GCComUser.UserName, GCComUser.AvatarUrl, GCComUser.PublicGuid, GCComUser.FindCount, GCComGeocacheLog.VisitDate, GCComGeocacheLog.Url")
                        .From("GCEuCCCUser with (nolock)")
                        .InnerJoin(string.Format("[{0}].[dbo].[GCComUser] with (nolock)", gcComDatabase)).On("GCEuCCCUser.GCComUserID=GCComUser.ID")
                        .InnerJoin(string.Format("[{0}].[dbo].[GCComGeocacheLog] with (nolock)", gcComDatabase)).On("GCComGeocacheLog.FinderId=GCEuCCCUser.GCComUserID")
                        .InnerJoin(string.Format("[{0}].[dbo].[yaf_User] with (nolock)", yafDatabase)).On("GCEuCCCUser.UserID=yaf_User.UserID")
                        .Where("GCComGeocacheLog.GeocacheID=@0 and GCEuCCCUser.Active=1 and GCComGeocacheLog.WptLogTypeId in (2, 10, 11)", gcInfo.ID)
                        .OrderBy("GCComGeocacheLog.VisitDate desc");
                    
                    var items = db.Page<CheckCCCItem>(page, pageSize, sql);
                    result.Items = items.Items.ToArray();
                    result.CurrentPage = items.CurrentPage;
                    result.PageCount = items.TotalPages;
                    result.TotalCount = items.TotalItems;

                    foreach (var it in result.Items)
                    {
                        if (removeEmailAddress || it.HideEmailAddress)
                        {
                            it.EMail = "";
                        }
                    }
                }
                else
                {
                    result.GeocacheCode = GCCode;
                    result.GeocacheTitle = "?";

                    result.Items = new CheckCCCItem[0];
                    result.CurrentPage = 1;
                    result.PageCount = 1;
                    result.TotalCount = 0;
                }
            }
            return result;
        }

        public string GetCCCServiceResult(string gccode, string username, string password)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                var m = new GCEuCCCRequest();
                m.UserName = username;
                m.GCCode = gccode;
                m.ValidPassword = false;
                m.ResultCount = 0;
                m.MemberCCC = false;
                m.GCComUserID = null;
                m.RequestAt = DateTime.Now;
                if (username != null && password != null)
                {
                    if (_membershipService.ValidateUser(username, password)!=null)
                    {
                        var settings = GetSettings(username);

                        m.MemberCCC = (settings != null && settings.Active);
                        m.ValidPassword = true;
                        if (settings != null)
                        {
                            m.GCComUserID = settings.GCComUserID;
                        }
                        if (gccode == null)
                        {
                            result.Append("OK");
                        }
                        else
                        {
                            if (settings != null && settings.Active)
                            {
                                result.AppendLine("BeginTitel");
                                string t = null;
                                using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                                {
                                    t = db.FirstOrDefault<string>("select top 1 Name from GCComGeocache where Code=@0", gccode);
                                }
                                if (!string.IsNullOrEmpty(t))
                                {
                                    result.AppendLine(t);
                                }
                                else
                                {
                                    result.AppendLine("Cache is unknown");
                                }
                                result.AppendLine("EndTitel");

                                var cccResult = GetCCCUsersForGeocache(1, 200, gccode, false);
                                if (cccResult != null)
                                {
                                    m.ResultCount = cccResult.Items.Length;
                                    if (cccResult.Owner != null)
                                    {
                                        m.ResultCount++;
                                        result.AppendLine("BeginRecord");
                                        result.AppendLine(string.Format("Name:{0} (CO)", cccResult.Owner.UserName??""));
                                        result.AppendLine(string.Format("Tel:{0}", cccResult.Owner.Telnr??""));
                                        result.AppendLine(string.Format("SMS:{0}", (cccResult.Owner.SMS) ? "1" : "0"));
                                        result.AppendLine(string.Format("SMS-Pref:{0}", (cccResult.Owner.PreferSMS) ? "1" : "0"));
                                        result.AppendLine(string.Format("Twitternaam:{0}", cccResult.Owner.TwitterUsername??""));
                                        result.AppendLine(string.Format("Remarks:{0}", cccResult.Owner.Comment??""));
                                        result.AppendLine(string.Format("LogDate:{0}", "2100-01-01"));
                                        if (!cccResult.Owner.HideEmailAddress)
                                        {
                                            result.AppendLine(string.Format("email:{0}", cccResult.Owner.EMail??""));
                                        }
                                        else
                                        {
                                            result.AppendLine("email:");
                                        }
                                        result.AppendLine("EndRecord");
                                    }
                                    foreach (var c in cccResult.Items)
                                    {
                                        result.AppendLine("BeginRecord");
                                        result.AppendLine(string.Format("Name:{0}", c.UserName ?? ""));
                                        result.AppendLine(string.Format("Tel:{0}", c.Telnr ?? ""));
                                        result.AppendLine(string.Format("SMS:{0}", (c.SMS) ? "1" : "0"));
                                        result.AppendLine(string.Format("SMS-Pref:{0}", (c.PreferSMS) ? "1" : "0"));
                                        result.AppendLine(string.Format("Twitternaam:{0}", c.TwitterUsername ?? ""));
                                        result.AppendLine(string.Format("Remarks:{0}", c.Comment ?? ""));
                                        result.AppendLine(string.Format("LogDate:{0}", c.VisitDate.ToString("yyyy-MM-dd")));
                                        if (!c.HideEmailAddress)
                                        {
                                            result.AppendLine(string.Format("email:{0}", c.EMail ?? ""));
                                        }
                                        else
                                        {
                                            result.AppendLine("email:");
                                        }
                                        result.AppendLine("EndRecord");
                                    }
                                }
                            }
                            else
                            {
                                result.Append("1");
                            }
                        }
                    }
                    else
                    {
                        result.Append("2");
                    }
                }
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    db.Insert(m);
                }
            }
            catch
            {
                result.Length = 0;
                result.Append("ERROR");
            }
            return result.ToString();
        }

        public GCEuCCCRequestModel GetRequestCalls(int page, int pageSize)
        {
            var result = new GCEuCCCRequestModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<GCEuCCCRequest>(page, pageSize, "select * from GCEuCCCRequest order by GCEuCCCRequest.ID desc");
                result.Calls = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.TotalCount = items.TotalItems;
                result.PageCount = items.TotalPages;
            }
            return result;
        }

    }
}
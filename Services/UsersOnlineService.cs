using Globalcaching.Core;
using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IUsersOnlineService : IDependency
    {
        OnlineUserInfo GetOnLineUsers();
    }

    public class UsersOnlineService : IUsersOnlineService
    {
        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public UsersOnlineService(IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        public OnlineUserInfo GetOnLineUsers()
        {
            OnlineUserInfo result = new OnlineUserInfo();
            result.Count = 0;
            result.Users = new List<OnlineUser>();
            GCEuUserSettings settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null)
            {
                using (var dbcon = new DBCon(DBCon.dbForumConnString))
                {
                    SqlDataReader dr = GetCurrentSiteVisitors(dbcon, 3600, true, HttpContext.Session.SessionID, settings.YafUserID);
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            OnlineUser ou = new OnlineUser();
                            ou.Count = (int)dr["aantal"];
                            ou.Donator = (bool)dr["Donator"];
                            ou.LastAccess = ((string)dr["lastaccess"]).Substring(11, 5);
                            ou.YafUserID = (int)dr["userid"];
                            ou.YafUserName = (string)dr["Name"];
                            ou.IsPosting = false;

                            object o = dr["LastForumPage"];
                            if (o != null && o.GetType() != typeof(DBNull))
                            {
                                //yaf_postmessage.aspx
                                if (((string)o).IndexOf("g=postmessage", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    ((string)o).IndexOf("yaf_postmessage.aspx", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    ou.IsPosting = true;
                                }
                            }

                            if (ou.YafUserID==1)
                            {
                                result.Count += ou.Count;
                            }
                            else
                            {
                                result.Count++;
                            }

                            result.Users.Add(ou);
                        }
                    }
                }
            }
            return result;
        }

        public SqlDataReader GetCurrentSiteVisitors(DBCon dbcon, int secondsThreshold, bool updateLastAccessTime, string sessionid, int userID)
        {
            SqlDataReader result = null;
            try
            {
                int useId = -1;
                if (updateLastAccessTime)
                {
                    useId = userID;
                    if (useId <= 0) useId = 1; //Guest
                }

                result = dbcon.GetCurrentSiteVisitors(secondsThreshold, useId, sessionid);
            }
            catch
            {
            }
            return result;
        }

    }
}
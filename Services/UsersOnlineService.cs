using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.ViewModels;
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
        List<UsersOnlineModel> GetUserActivity();
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

        public List<UsersOnlineModel> GetUserActivity()
        {
            List<UsersOnlineModel> result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(DBCon.dbForumConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<UsersOnlineModel>("select yaf_User.Name, yaf_User.UserID, LastPageAccess.IP, LastPageAccess.LastActive, LastPageAccess.Location, LastPageAccess.PageParam from yaf_User inner join LastPageAccess on LastPageAccess.UserID=yaf_User.UserID where LastPageAccess.BoardID=1 order by LastActive desc");
            }
            return result;
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
                    UpdateLastPageAccess(dbcon, HttpContext.Request.UserHostAddress??"", HttpContext.Request.RawUrl??"", settings.YafUserID);
                    SqlDataReader dr = GetCurrentSiteVisitors(dbcon, 3600, true, HttpContext.Request.UserHostAddress??"", settings.YafUserID);
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

        public static void UpdateLastPageAccess(DBCon dbcon, string ip, string rawUrl, int userID)
        {
            try
            {
                int userId = userID;
                if (userId <= 0)
                {
                    userId = 1; //Guest
                }
                string page;
                string pageParam;
                int pos = rawUrl.IndexOf('?');
                if (pos > 0 && pos < (rawUrl.Length - 1))
                {
                    page = rawUrl.Substring(0, pos);
                    pageParam = rawUrl.Substring(pos + 1);
                }
                else
                {
                    page = rawUrl;
                    pageParam = "";
                }
                int boardID = 1;

                dbcon.ExecuteNonQuery(string.Format("delete from LastPageAccess where LastActive < '{0}'", DateTime.Now.AddMinutes(-60).ToString("u")));
                if (userId == 1)
                {
                    if ((int)dbcon.ExecuteScalar(string.Format("select count(1) from LastPageAccess where IP='{0}' and BoardID={1}", ip, boardID)) > 0)
                    {
                        dbcon.ExecuteNonQuery(string.Format("update LastPageAccess set UserID={4}, LastActive='{1}', Location='{2}', PageParam='{3}' where IP='{0}' and BoardID={5}", ip, DateTime.Now.ToString("u"), page.Replace("'", "''"), pageParam.Replace("'", "''"), userId, boardID));
                    }
                    else
                    {
                        dbcon.ExecuteNonQuery(string.Format("insert into LastPageAccess (IP, LastActive, Location, PageParam, UserID, BoardID) values ('{0}', '{1}', '{2}', '{3}', {4}, {5})", ip, DateTime.Now.ToString("u"), page.Replace("'", "''"), pageParam.Replace("'", "''"), userId, boardID));
                    }
                }
                else
                {
                    if ((int)dbcon.ExecuteScalar(string.Format("select count(1) from LastPageAccess where UserID={0} and BoardID={1}", userId, boardID)) > 0)
                    {
                        dbcon.ExecuteNonQuery(string.Format("update LastPageAccess set IP='{0}', LastActive='{1}', Location='{2}', PageParam='{3}' where UserID={4} and BoardID={5}", ip, DateTime.Now.ToString("u"), page.Replace("'", "''"), pageParam.Replace("'", "''"), userId, boardID));
                    }
                    else
                    {
                        dbcon.ExecuteNonQuery(string.Format("insert into LastPageAccess (IP, LastActive, Location, PageParam, UserID, BoardID) values ('{0}', '{1}', '{2}', '{3}', {4}, {5})", ip, DateTime.Now.ToString("u"), page.Replace("'", "''"), pageParam.Replace("'", "''"), userId, boardID));
                    }
                }
            }
            catch
            {
            }
        }
    }
}
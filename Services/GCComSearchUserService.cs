using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGCComSearchUserService : IDependency
    {
        GCComUser GetGeocachingComUser(string name);
        GCComUser GetGeocachingComUser(long userId);
        GCComUserSearchResult GetGeocachingComUsers(int mode, int page, int pageSize, string name);
        GeocacherInfoModel GetGeocacherInfo(long userId);
    }

    public class GCComSearchUserService : IGCComSearchUserService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GCComSearchUserService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public GCComUser GetGeocachingComUser(string name)
        {
            GCComUser result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = db.FirstOrDefault<GCComUser>("where UserName=@0", name);
            }
            return result;
        }

        public GCComUser GetGeocachingComUser(long userId)
        {
            GCComUser result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = db.FirstOrDefault<GCComUser>("where ID=@0", userId);
            }
            return result;
        }

        public GCComUserSearchResult GetGeocachingComUsers(int mode, int page, int pageSize, string name)
        {
            GCComUserSearchResult result = new GCComUserSearchResult();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                if (mode != 2)
                {
                    result.Current = new GCComUserSearchListResult();
                    result.Current.PageCount = 1;
                    result.Current.CurrentPage = 1;
                    var items = db.Page<GCComUser>(page, pageSize, string.Format("Where  UserName like '%{0}%' order by UserName", (name ?? "").Replace("@", "@@").Replace("'", "''")));
                    result.Current.Users = items.Items.ToArray();
                    result.Current.CurrentPage = items.CurrentPage;
                    result.Current.PageCount = items.TotalPages;
                    result.Current.TotalCount = items.TotalItems;
                }
                if (mode != 1)
                {
                    result.History = new GCComUserSearchListResult();
                    result.History.PageCount = 1;
                    result.History.CurrentPage = 1;
                    var items = db.Page<GCComUser>(page, pageSize, string.Format("select * from (select distinct GCComUser.* from GCEuData.dbo.GCEuComUserNameChange inner join GCComUser on GCEuComUserNameChange.ID = GCComUser.ID and (GCEuComUserNameChange.OldName like '%{0}%' or GCEuComUserNameChange.NewName like '%{0}%')) as A order by UserName", (name ?? "").Replace("@", "@@").Replace("'", "''")));
                    result.History.Users = items.Items.ToArray();
                    result.History.CurrentPage = items.CurrentPage;
                    result.History.PageCount = items.TotalPages;
                    result.History.TotalCount = items.TotalItems;
                }
            }
            return result;
        }

        public GeocacherInfoModel GetGeocacherInfo(long userId)
        {
            var result = new GeocacherInfoModel();
            result.GCComUser = GetGeocachingComUser(userId);
            result.GCEuUserSettings = _gcEuUserSettingsService.GetSettings(userId);
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.GCComNameChanges = db.Fetch<GCEuComUserNameChange>("where ID=@0", userId).OrderBy(x => x.DetectedAt).ToList();
                if (result.GCEuUserSettings != null)
                {
                    result.GCEuUserName = db.ExecuteScalar<string>("select Name from Globalcaching.dbo.yaf_User where UserID=@0", result.GCEuUserSettings.YafUserID);
                }
            }
            return result;
        }

    }
}
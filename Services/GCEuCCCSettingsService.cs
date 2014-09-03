using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGCEuCCCSettingsService : IDependency
    {
        GCEuCCCUser GetSettings();
        void UpdateSettings(GCEuCCCUser settings);
        CheckCCCResult GetCCCUsersForGeocache(int page, int pageSize, string GCCode, bool removeEmailAddress);
    }

    public class GCEuCCCSettingsService : IGCEuCCCSettingsService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IOrchardServices _orchardServices;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GCEuCCCSettingsService(IWorkContextAccessor workContextAccessor,
            IOrchardServices orchardServices,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _workContextAccessor = workContextAccessor;
            _orchardServices = orchardServices;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }


        public GCEuCCCUser GetSettings()
        {
            GCEuCCCUser result = null;
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    result = db.SingleOrDefault<GCEuCCCUser>("where UserID = @0", settings.YafUserID);
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
                    if (db.SingleOrDefault<GCEuCCCUser>("where UserID = @0", settings.UserID) != null)
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

    }
}
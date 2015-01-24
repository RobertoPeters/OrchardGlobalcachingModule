using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGeocacheBatchLogService: IDependency
    {
        List<GeocacheBatchLogModel> GetGeocachesByOwner(string owner);
        List<GeocacheBatchLogModel> GetGeocachesByCode(string code);
        List<GeocacheBatchLogModel> GetGeocachesByName(string name);
    }

    public class GeocacheBatchLogService : IGeocacheBatchLogService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GeocacheBatchLogService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public List<GeocacheBatchLogModel> GetGeocachesByOwner(string owner)
        {
            List<GeocacheBatchLogModel> result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<GeocacheBatchLogModel>("select GCComGeocache.ID, GCComGeocache.Code, GCComGeocache.Name from GCComUser inner join GCComGeocache on GCComUser.ID = GCComGeocache.OwnerId where GCComUser.UserName=@0 and GCComGeocache.Archived=0", owner);
                checkFounds(db, result);
            }
            return result;
        }

        public List<GeocacheBatchLogModel> GetGeocachesByName(string name)
        {
            List<GeocacheBatchLogModel> result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<GeocacheBatchLogModel>(string.Format("select top 200 ID, Code, Name from GCComGeocache where Name like '%{0}%'", name.Replace("'", "''").Replace("@", "@@")));
                checkFounds(db, result);
            }
            return result;
        }

        public List<GeocacheBatchLogModel> GetGeocachesByCode(string code)
        {
            List<GeocacheBatchLogModel> result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<GeocacheBatchLogModel>("select ID, Code, Name from GCComGeocache where Code=@0", code);
                checkFounds(db, result);
            }
            return result;
        }

        private void checkFounds(PetaPoco.Database db, List<GeocacheBatchLogModel> items)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.GCComUserID != null)
            {
                foreach (var item in items)
                {
                    item.Found = db.Fetch<long>("select top 1 ID from GCComGeocacheLog where GeocacheID=@0 and FinderId=@1 and WptLogTypeId in (2, 10, 11)", item.ID, settings.GCComUserID).Count() > 0;
                }
            }
        }

    }
}
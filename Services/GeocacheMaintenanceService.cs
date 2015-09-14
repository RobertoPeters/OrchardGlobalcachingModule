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
    public interface IGeocacheMaintenanceService : IDependency
    {
        GeocacheMaintenanceModel GetGeocacheMaintenanceInfo(string ownerName);
        GeocacheMaintenanceModel GetGeocacheMaintenanceInfo(long? gcComUserID);
    }

    public class GeocacheMaintenanceService : IGeocacheMaintenanceService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        private readonly IGCComSearchUserService _gcComSearchUserService;

        public GeocacheMaintenanceService(IGCComSearchUserService gcComSearchUserService)
        {
            _gcComSearchUserService = gcComSearchUserService;
        }

        public GeocacheMaintenanceModel GetGeocacheMaintenanceInfo(string ownerName)
        {
            var gcComUser = _gcComSearchUserService.GetGeocachingComUser(ownerName);
            if (gcComUser != null)
            {
                return GetGeocacheMaintenanceInfo(gcComUser);
            }
            else
            {
                return null;
            }
        }

        public GeocacheMaintenanceModel GetGeocacheMaintenanceInfo(long? gcComUserID)
        {
            if (gcComUserID==null || !gcComUserID.HasValue) return null;
            return GetGeocacheMaintenanceInfo(_gcComSearchUserService.GetGeocachingComUser((long)gcComUserID));
        }

        private GeocacheMaintenanceModel GetGeocacheMaintenanceInfo(GCComUser gcComUser)
        {
            if (gcComUser == null) return null;
            GeocacheMaintenanceModel result = new GeocacheMaintenanceModel();
            result.UserName = gcComUser.UserName;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result.Geocaches = db.Fetch<GeocacheMaintenanceGeocacheInfo>(@"select a.*,
(select count(1) from GCComGeocacheLog where GCComGeocacheLog.GeocacheID=a.ID and GCComGeocacheLog.WptLogTypeId=2 and GCComGeocacheLog.VisitDate> LastOwnerMaintenance) as LogsMaintenance
from
(select GCComGeocache.ID,
GCComGeocache.Code,
GCComGeocache.ContainerTypeId,
GCComGeocache.GeocacheTypeId,
GCComGeocache.Url,
COALESCE(GCEuGeocache.PublishedAtDate, GCComGeocache.UTCPlaceDate) as PublishedDate,
GCEuGeocache.FoundCount,
(select MAX(GCComGeocacheLog.VisitDate) from GCComGeocacheLog where GCComGeocacheLog.GeocacheID=GCComGeocache.ID and GCComGeocacheLog.WptLogTypeId=46) as LastOwnerMaintenance,
(select Count(1) from GCComGeocacheLog where GCComGeocacheLog.GeocacheID=GCComGeocache.ID and GCComGeocacheLog.WptLogTypeId=46) as CountMaintenance
from GCComGeocache
inner join GCEuData.dbo.GCEuGeocache on  GCComGeocache.ID = GCEuGeocache.ID
where GCComGeocache.OwnerId=@0 and GCComGeocache.Archived=0) a
order by a.ContainerTypeId, LogsMaintenance desc", gcComUser.ID);
            }
            return result;
        }

    }
}
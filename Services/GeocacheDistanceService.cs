using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGeocacheDistanceService : IDependency
    {
        GeocacheDistanceAdminModel GetUnassignedDistance(int page, int pageSize);
        bool SetDistanceChecked(long id);
        bool SetDistance(long id, double? distance);
    }

    public class GeocacheDistanceService : IGeocacheDistanceService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public GeocacheDistanceService()
        {
        }

        public GeocacheDistanceAdminModel GetUnassignedDistance(int page, int pageSize)
        {
            GeocacheDistanceAdminModel result = new GeocacheDistanceAdminModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<GeocacheDistanceAdminInfo>(page, pageSize, "select GCEuGeocache.ID, GCEuGeocache.PublishedAtDate, GCComGeocache.Code, GCComGeocache.Name, GCComGeocacheType.GeocacheTypeName from GCEuGeocache inner join GCComData.dbo.GCComGeocache on GCComGeocache.ID = GCEuGeocache.ID inner join GCComData.dbo.GCComGeocacheType on GCComGeocacheType.ID = GCComGeocache.GeocacheTypeId where Distance is null and GCEuGeocache.DistanceChecked = 0 and GCComGeocache.CountryID = 141 order by GCEuGeocache.PublishedAtDate");
                result.Geocaches = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.TotalCount = items.TotalItems;
                result.PageCount = items.TotalPages;
            }
            return result;
        }

        public bool SetDistance(long id, double? distance)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                if (distance == null)
                {
                    result = db.Execute("update GCEuGeocache set Distance = NULL, DistanceChecked=1 where ID=@0", id) > 0;
                }
                else
                {
                    result = db.Execute("update GCEuGeocache set Distance = @0, DistanceChecked=1 where ID=@1", (double)distance, id) > 0;
                }
            }
            return result;
        }

        public bool SetDistanceChecked(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set DistanceChecked=1 where ID=@0", id) > 0;
            }
            return result;
        }

    }
}
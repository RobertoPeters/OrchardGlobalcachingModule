using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGeocacheSeriesService : IDependency
    {
        GeocacheSeriesModel GetGeocacheSeries(int page, int pageSize, GeocacheSeriesFilter filter);
    }

    public class GeocacheSeriesService : IGeocacheSeriesService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public GeocacheSeriesModel GetGeocacheSeries(int page, int pageSize, GeocacheSeriesFilter filter)
        {
            GeocacheSeriesModel result = new GeocacheSeriesModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Filter = filter;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                //var items = db.Fetch<GeocacheSeriesInfo>("select Left(Name,@0)+'...'+Right(Name,@1) as NameMatch, COUNT(1) as NumberOfCaches from GCComGeocache where Archived=0 AND GCComGeocache.CountryID=@2 group by Left(Name,@0)+'...'+Right(Name,@1) HAVING count(*) > 2 order by NumberOfCaches desc", filter.BeginLength, filter.EndLength, filter.CountryID);
                var items = db.Fetch<GeocacheSeriesInfo>(string.Format("select Left(Name,{0})+'...'+Right(Name,{1}) as NameMatch, COUNT(1) as NumberOfCaches from GCComGeocache where Archived=0 AND GCComGeocache.CountryID={2} group by Left(Name,{0})+'...'+Right(Name,{1}) HAVING count(*) > 2 order by NumberOfCaches desc", filter.BeginLength, filter.EndLength, filter.CountryID));
                result.Geocaches = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                result.CurrentPage = page;
                result.TotalCount = items.Count();
                result.PageCount = result.TotalCount / pageSize;
                if (result.TotalCount > 0 && (result.TotalCount % pageSize) != 0)
                {
                    result.PageCount++;
                }
            }
            return result;
        }
    }
}
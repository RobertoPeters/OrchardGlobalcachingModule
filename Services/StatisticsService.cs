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
    public interface IStatisticsServiceService : IDependency
    {
        List<StatisticsGeocachesPerYear> GetGeocachesPerYear(bool inclArchived, int countryId);
        List<GeocacheTypeStats> GetGeocacheTyeStats();
    }

    public class StatisticsService : IStatisticsServiceService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public List<GeocacheTypeStats> GetGeocacheTyeStats()
        {
            List<GeocacheTypeStats> result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = @"select GCComGeocacheType.GeocacheTypeName, b.* from 
(select GCComGeocache.GeocacheTypeId, COUNT(1) as CacheCount, SUM(GCEuGeocache.FoundCount) as FoundCount, SUM(GCEuGeocache.PMFoundCount) as PMFoundCount, SUM(GCEuGeocache.LogImageCount) as LogImageCount, SUM(GCComGeocache.FavoritePoints) as FavoritePoints, SUM(DATEDIFF(DAY, GCEuGeocache.PublishedAtDate, GETDATE())) as DaysOnline from GCComGeocache with (nolock) inner join GCEuData.dbo.GCEuGeocache on GCComGeocache.ID = GCEuGeocache.ID  where GCComGeocache.Available=1 group by GCComGeocache.GeocacheTypeId) as b
inner join GCComGeocacheType with (nolock) on b.GeocacheTypeId = GCComGeocacheType.ID
where b.GeocacheTypeId not in (4, 6, 11, 12, 13, 31, 453)";
                result = db.Fetch<GeocacheTypeStats>(sql);
            }
            return result;
        }

        public List<StatisticsGeocachesPerYear> GetGeocachesPerYear(bool inclArchived, int countryId)
        {
            List<StatisticsGeocachesPerYear> result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                string arc;
                if (inclArchived) arc = "";
                else arc = "Archived=0 and ";
                var sql = string.Format(@"select Jaar, [2] as _2, [3] as _3, [3] as _3, [4] as _4, [5] as _5, [6] as _6, [8] as _8, [11] as _11, [13] as _13, [137] as _137, [453] as _453, [1858] as _1858 from
                                (select CAST(Year(UTCPlaceDate) as NVARCHAR) as Jaar, GeocacheTypeId from GCComGeocache with (nolock) 
                                where {1} CountryID={0})
                                p 
                                pivot
                                (
                                count(GeocacheTypeId)
                                for GeocacheTypeId in ([2], [3], [4], [5], [6], [8], [11], [13], [137], [453], [1858])
                                ) as pvt
                                union
                                select Jaar = 'Totaal', [2] as _2, [3] as _3, [3] as _3, [4] as _4, [5] as _5, [6] as _6, [8] as _8, [11] as _11, [13] as _13, [137] as _137, [453] as _453, [1858] as _1858 from
                                (select GeocacheTypeId from GCComGeocache with (nolock)
                                where {1} CountryID={0})
                                p2
                                pivot
                                (
                                count(GeocacheTypeId)
                                for GeocacheTypeId in ([2], [3], [4], [5], [6], [8], [11], [13], [137], [453], [1858])
                                ) as pvt2
                                order by Jaar
                                ", countryId, arc);
                result = db.Fetch<StatisticsGeocachesPerYear>(sql);
            }
            return result;
        }
    }
}
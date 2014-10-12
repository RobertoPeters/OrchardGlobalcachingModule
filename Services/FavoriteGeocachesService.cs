using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IFavoriteGeocachesService : IDependency
    {
        FavoriteGeocachesModel GetFavoriteGeocaches(int page, int pageSize, FavoriteGeocacheFilter filter);
    }

    public class FavoriteGeocachesService : IFavoriteGeocachesService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public FavoriteGeocachesService()
        {
        }

        public FavoriteGeocachesModel GetFavoriteGeocaches(int page, int pageSize, FavoriteGeocacheFilter filter)
        {
            FavoriteGeocachesModel result = new FavoriteGeocachesModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Filter = filter;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder.Append("select GCComGeocache.Code, GCComGeocache.Latitude, GCComGeocache.Longitude, GCComGeocache.Name, GCComGeocache.Archived, GCComGeocache.Available, GCComGeocache.GeocacheTypeId, GCComGeocache.OwnerId, GCComGeocache.ContainerTypeId, GCComGeocache.FavoritePoints, GCComUser.UserName, GCComUser.PublicGuid, GCComUser.AvatarUrl, DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()) as DaysOnline, 'FavPer100Found' = CASE WHEN GCEuGeocache.FoundCount=0 THEN 0 ELSE 100*CONVERT(FLOAT,GCComGeocache.FavoritePoints)/CONVERT(FLOAT,GCEuGeocache.FoundCount) END from GCComGeocache");
                sql = sql.InnerJoin("GCComUser").On("GCComGeocache.OwnerId = GCComUser.ID");
                sql = sql.InnerJoin("[GCEuData].[dbo].[GCEuGeocache]").On("GCComGeocache.ID = GCEuGeocache.ID");
                sql = sql.Where("GCComGeocache.Archived=0");
                sql = sql.Append("and GCComGeocache.CountryId = @0", filter.CountryId);
                sql = sql.Append("and DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()) >= @0", filter.MinDaysOnline);
                sql = sql.Append("and GCComGeocache.FavoritePoints >= @0", filter.MinFavorites);
                sql = sql.Append("and GCEuGeocache.FoundCount >= @0", filter.MinFoundCount);
                var items = db.Page<FavoriteGeocacheInfo>(page, pageSize, sql);
                result.FavoriteGeocaches = items.Items.ToList();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }

    }
}
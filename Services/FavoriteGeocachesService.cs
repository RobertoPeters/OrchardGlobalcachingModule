using Gavaghan.Geodesy;
using Globalcaching.Core;
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
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public FavoriteGeocachesService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public FavoriteGeocachesModel GetFavoriteGeocaches(int page, int pageSize, FavoriteGeocacheFilter filter)
        {
            FavoriteGeocachesModel result = new FavoriteGeocachesModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Filter = filter;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder.Append("select GCComGeocache.ID, GCComGeocache.Code, GCComGeocache.Latitude, GCComGeocache.Longitude, GCComGeocache.Name, GCComGeocache.Archived, GCComGeocache.Available, GCComGeocache.GeocacheTypeId, GCComGeocache.OwnerId, GCComGeocache.ContainerTypeId, GCComGeocache.FavoritePoints, GCComGeocache.Url, GCComUser.UserName, GCComUser.PublicGuid, GCComUser.AvatarUrl, GCEuGeocache.FoundCount, GCEuGeocache.PMFoundCount, DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()) as DaysOnline, GCEuGeocache.FavPer100Found from GCComGeocache");
                sql = sql.InnerJoin("GCComUser").On("GCComGeocache.OwnerId = GCComUser.ID");
                sql = sql.InnerJoin("[GCEuData].[dbo].[GCEuGeocache]").On("GCComGeocache.ID = GCEuGeocache.ID");
                sql = sql.Where("GCComGeocache.Archived=0");
                sql = sql.Append("and GCComGeocache.CountryId = @0", filter.CountryId);
                sql = sql.Append("and DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()) >= @0", filter.MinDaysOnline);
                sql = sql.Append("and GCComGeocache.FavoritePoints >= @0", filter.MinFavorites);
                sql = sql.Append("and GCEuGeocache.FoundCount >= @0", filter.MinFoundCount);
                if (filter.SortOn == 0)
                {
                    sql = sql.Append("order by FavoritePoints desc");
                }
                else
                {
                    sql = sql.Append("order by FavPer100Found desc");
                }
                var items = db.Page<FavoriteGeocacheInfo>(page, pageSize, sql);
                result.FavoriteGeocaches = items.Items.ToList();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;

                var settings = _gcEuUserSettingsService.GetSettings();
                foreach (var item in result.FavoriteGeocaches)
                {
                    if (settings != null && settings.HomelocationLat != null && settings.HomelocationLon != null && item.Latitude != null && item.Longitude != null)
                    {
                        GeodeticMeasurement gm = Helper.CalculateDistance((double)settings.HomelocationLat, (double)settings.HomelocationLon, (double)item.Latitude, (double)item.Longitude);
                        item.DirectionIcon = Helper.GetWindDirection(gm.Azimuth);
                        item.DistanceFromHome = gm.EllipsoidalDistance / 1000.0;

                    }
                    if (settings != null && settings.GCComUserID != null)
                    {
                        item.Own = item.OwnerId == settings.GCComUserID;
                        item.Found = db.Fetch<long>("select top 1 ID from GCComGeocacheLog where GeocacheID=@0 and FinderId=@1 and WptLogTypeId in (2, 10, 11)", item.ID, settings.GCComUserID).Count()>0;
                    }
                    else
                    {
                        item.Found = false;
                        item.Own = false;
                    }
                }

            }
            return result;
        }

    }
}
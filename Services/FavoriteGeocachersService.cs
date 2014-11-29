using Gavaghan.Geodesy;
using Globalcaching.Core;
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
    public interface IFavoriteGeocachersService : IDependency
    {
        FavoriteGeocachersModel GetFavoriteGeocachers(int page, int pageSize, FavoriteGeocacherFilter filter);
    }

    public class FavoriteGeocachersService : IFavoriteGeocachersService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public FavoriteGeocachersService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public FavoriteGeocachersModel GetFavoriteGeocachers(int page, int pageSize, FavoriteGeocacherFilter filter)
        {
            FavoriteGeocachersModel result = new FavoriteGeocachersModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Filter = filter;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder.Append("select GCComGeocache.OwnerId, Count(1) as CacheCount, AVG(GCComGeocache.Latitude) as Latitude, AVG(GCComGeocache.Longitude) as Longitude, SUM(GCEuGeocache.FoundCount) as FoundCount, SUM(GCComGeocache.FavoritePoints) as FavoriteCount, SUM(DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE())) as DaysOnline from GCComGeocache");
                sql = sql.InnerJoin("GCComUser").On("GCComGeocache.OwnerId = GCComUser.ID");
                sql = sql.InnerJoin("[GCEuData].[dbo].[GCEuGeocache]").On("GCComGeocache.ID = GCEuGeocache.ID");
                sql = sql.Where("GCComGeocache.Archived=0");
                sql = sql.Append("and GCComGeocache.CountryId = @0", filter.CountryId);
                sql = sql.Append("and DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()) >= @0", filter.CacheMinDaysOnline);
                sql = sql.Append("and GCComGeocache.FavoritePoints >= @0", filter.CacheMinFavorites);
                sql = sql.Append("and GCEuGeocache.FoundCount >= @0", filter.CacheMinFoundCount);
                if (!string.IsNullOrWhiteSpace(filter.UserName))
                {
                    sql = sql.Append(string.Format("and GCComUser.UserName LIKE '%{0}%'", filter.UserName.Replace("@", "@@").Replace("'", "''")));
                }
                sql = sql.GroupBy("GCComGeocache.OwnerId");
                sql = sql.Append("having SUM(GCEuGeocache.FoundCount)>=@0", filter.TotalMinFoundCount);
                sql = sql.Append("and SUM(GCComGeocache.FavoritePoints)>=@0", filter.TotalMinFavorites);
                sql = sql.Append("and SUM(DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()))>=@0", filter.TotalMinDaysOnline);

                var items = db.Fetch<FavoriteGeocacherInfo>(sql);
                foreach (var item in items)
                {
                    if (item.FoundCount > 0)
                    {
                        item.FavPer100Found = 100.0 * (double)item.FavoriteCount / (double)item.FoundCount;
                    }
                    else
                    {
                        item.FavPer100Found = 0;
                    }
                }
                if (filter.SortOn == 0)
                {
                    result.FavoriteGeocachers = items.OrderByDescending(x => x.FoundCount).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    result.FavoriteGeocachers = items.OrderByDescending(x => x.FavPer100Found).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }

                result.CurrentPage = page;
                result.TotalCount = items.Count();
                result.PageCount = result.TotalCount / pageSize;
                if (result.TotalCount > 0 && (result.TotalCount % pageSize) != 0)
                {
                    result.PageCount++;
                }

                var settings = _gcEuUserSettingsService.GetSettings();
                foreach (var item in result.FavoriteGeocachers)
                {
                    var gcUser = db.FirstOrDefault<GCComUser>("where ID=@0", item.OwnerId);
                    item.UserName = gcUser.UserName;
                    item.PublicGuid = gcUser.PublicGuid;
                    item.AvatarUrl = gcUser.AvatarUrl;
                    if (settings != null && settings.HomelocationLat != null && settings.HomelocationLon != null && item.Latitude != null && item.Longitude != null)
                    {
                        GeodeticMeasurement gm = Helper.CalculateDistance((double)settings.HomelocationLat, (double)settings.HomelocationLon, (double)item.Latitude, (double)item.Longitude);
                        item.DirectionIcon = Helper.GetWindDirection(gm.Azimuth);
                        item.DistanceFromHome = gm.EllipsoidalDistance / 1000.0;

                    }
                }
            }
            return result;
        }

    }
}
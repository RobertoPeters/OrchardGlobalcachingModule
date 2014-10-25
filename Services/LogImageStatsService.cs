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
    public interface ILogImageStatsService : IDependency
    {
        LogImageStatsModel GetLogImageStats(int page, int pageSize, LogImageStatsFilter filter);
    }

    public class LogImageStatsService : ILogImageStatsService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public LogImageStatsService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public LogImageStatsModel GetLogImageStats(int page, int pageSize, LogImageStatsFilter filter)
        {
            LogImageStatsModel result = new LogImageStatsModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Filter = filter;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                //SavedQuery = string.Format("select top {0} Waypoint, Owner, Lat, Lon, CacheType, Description, FoundCount, ImageCount, HiddenDate, DATEDIFF(DAY,CONVERT(DATETIME,HiddenDate),GETDATE()) as DaysOnline, 'ImgPer100Found' = CASE WHEN ImageCount=0 THEN 0 ELSE 100*CONVERT(FLOAT,ImageCount)/CONVERT(FLOAT,FoundCount) END from Caches where Archived=0 and DATEDIFF(DAY,CONVERT(DATETIME,HiddenDate),GETDATE()) >= {1} and ImageCount >= {2} and FoundCount >= {3} and Country='{4}' and CacheType='{6}' order by {5} desc", MaxFavCache.Text.Replace("'", "''"), TextBox1.Text.Replace("'", "''"), TextBox2.Text.Replace("'", "''"), TextBox3.Text.Replace("'", "''"), cntry, sortOn, DropDownList2.SelectedValue.Replace("'", "''"));
                var sql = PetaPoco.Sql.Builder.Append("select GCComGeocache.ID, GCComGeocache.Code, GCComGeocache.Latitude, GCComGeocache.Longitude, GCComGeocache.Name, GCComGeocache.GeocacheTypeId, GCComGeocache.OwnerId, GCComGeocache.Url, GCComUser.UserName, GCComUser.PublicGuid, GCEuGeocache.FoundCount, DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()) as DaysOnline, GCEuGeocache.LogImageCount, GCEuGeocache.LogImagePer100Found from GCComGeocache");
                sql = sql.InnerJoin("GCComUser").On("GCComGeocache.OwnerId = GCComUser.ID");
                sql = sql.InnerJoin("[GCEuData].[dbo].[GCEuGeocache]").On("GCComGeocache.ID = GCEuGeocache.ID");
                sql = sql.Where("GCComGeocache.Archived=0");
                sql = sql.Append("and GCComGeocache.CountryId = @0", filter.CountryId);
                sql = sql.Append("and DATEDIFF(DAY,GCComGeocache.UTCPlaceDate,GETDATE()) >= @0", filter.MinDaysOnline);
                sql = sql.Append("and GCEuGeocache.LogImageCount >= @0", filter.MinImageCount);
                sql = sql.Append("and GCEuGeocache.FoundCount >= @0", filter.MinFoundCount);
                sql = sql.Append("and GCComGeocache.GeocacheTypeId = @0", filter.CacheTypeId);
                if (filter.SortOn == 0)
                {
                    sql = sql.Append("order by FavoritePoints desc");
                }
                else
                {
                    sql = sql.Append("order by FavPer100Found desc");
                }
                var items = db.Page<LogImageStatsInfo>(page, pageSize, sql);
                result.Geocaches = items.Items.ToList();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;

                var settings = _gcEuUserSettingsService.GetSettings();
                foreach (var item in result.Geocaches)
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
                    item.LogImages = db.Fetch<LogImageInfo>("select top 4 GCComGeocacheLogImage.ThumbUrl, GCComGeocacheLogImage.Url from GCComGeocacheLogImage with (nolock) inner join GCComGeocacheLog with (nolock) on GCComGeocacheLogImage.LogID = GCComGeocacheLog.ID where GCComGeocacheLog.GeocacheID=@0 order by GCComGeocacheLog.UTCCreateDate desc", item.ID);
                }

            }
            return result;
        }

    }
}
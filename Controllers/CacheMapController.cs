using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Globalcaching.Controllers
{
    public class CacheMapController: Controller
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public CacheMapController(IGCEuUserSettingsService gcEuUserSettingsService,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index()
        {
            return ShowMap(new GeocacheSearchFilter());
        }

        [Themed]
        public ActionResult MacroResult()
        {
            return ShowMap(new GeocacheSearchFilter() { MacroResult = true });
        }

        [Themed]
        public ActionResult ShowMap(GeocacheSearchFilter filter)
        {
            CacheMapSettings mapSettings = new CacheMapSettings();
            mapSettings.CenterLat = 50.5;
            mapSettings.CenterLon = 5.5;
            mapSettings.InitialZoomLevel = 12;
            mapSettings.Filter = filter;

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                if (filter.CenterLat != null && filter.CenterLon != null)
                {
                    mapSettings.CenterLat = (double)filter.CenterLat;
                    mapSettings.CenterLon = (double)filter.CenterLon;
                }
                else
                {
                    var sql = PetaPoco.Sql.Builder;
                    if (filter.MaxResult > 0)
                    {
                        sql.Append(string.Format("select top {0} GCComGeocache.Latitude as a, GCComGeocache.Longitude as o"));
                        addWhereClause(sql, filter, null, null, null, null);
                        var items = db.Fetch<GeocacheMapInfo>(sql);
                        if (items.Count > 0)
                        {
                            mapSettings.CenterLat = items.Average(x => x.a);
                            mapSettings.CenterLon = items.Average(x => x.o);
                        }
                    }
                    else
                    {
                        sql.Append("select Avg(GCComGeocache.Latitude) as a, Avg(GCComGeocache.Longitude) as o");
                        addWhereClause(sql, filter, null, null, null, null);
                        var items = db.Fetch<GeocacheMapInfo>(sql);
                        if (items.Count > 0)
                        {
                            mapSettings.CenterLat = items[0].a;
                            mapSettings.CenterLon = items[0].o;
                        }
                    }
                }
            }

            return View("ShowMap", mapSettings);
        }

        public class GeocacheMapInfo
        {
            public double a { get; set; } //Lat
            public double o { get; set; } //Lon
        }

        public class GeocacheMapItem
        {
            public string c { get; set; } //code
            public string i { get; set; } //icon
            public double a { get; set; } //Lat
            public double o { get; set; } //Lon

            [ScriptIgnore]
            public long OwnerId { get; set; }
            [ScriptIgnore]
            public long? FoundLogID { get; set; }
        }
        public class GeocacheMapResult
        {
            public GeocacheMapItem[] Items { get; set; }
        }

        public class WaypointInfo
        {
            public string Code { get; set; }
            public int GeocacheTypeId { get; set; }

        }

        [HttpPost]
        public ActionResult GetWaypointInfo(string code)
        {
            WaypointInfo wpi = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder
                    .Select("GCComGeocache.Code", "GCComGeocache.GeocacheTypeId")
                    .From("GCComGeocache with (nolock)")
                    .Where("GCComGeocache.Code=@0", code);

                wpi = db.FirstOrDefault<WaypointInfo>(sql);
            }
            return Json(wpi);
        }

        [HttpPost]
        public ActionResult SearchGeocaches(GeocacheSearchFilter filter, double minLat, double minLon, double maxLat, double maxLon, int zoom)
        {
            return Json(GetGeocaches(filter, minLat, minLon, maxLat, maxLon, zoom));
        }

        private PetaPoco.Sql addWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter, double? minLat, double? minLon, double? maxLat, double? maxLon)
        {
            string euDatabase = Core.Helper.GetTableNameFromConnectionString(dbGcEuDataConnString);
            sql = sql.From("GCComGeocache with (nolock)")
                .InnerJoin(string.Format("[{0}].[dbo].[GCEuGeocache]", euDatabase)).On("GCComGeocache.ID = GCEuGeocache.ID")
                .InnerJoin(string.Format("GCComUser", euDatabase)).On("GCComGeocache.OwnerId = GCComUser.ID");
            if (filter.MacroResult != null)
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                sql = sql.InnerJoin(string.Format("GCEuMacroData.dbo.macro_{0}_Resultaat", settings.YafUserID)).On(string.Format("GCComGeocache.ID = GCEuMacroData.dbo.macro_{0}_Resultaat.ID", settings.YafUserID));
                if (minLat != null)
                {
                    sql = sql.Where("GCComGeocache.Latitude>=@0", minLat);
                    sql = sql.Append("and GCComGeocache.Latitude<=@0", maxLat);
                    sql = sql.Append("and GCComGeocache.Longitude>=@0", minLon);
                    sql = sql.Append("and GCComGeocache.Longitude<=@0", maxLon);
                }
            }
            else
            {
                sql = sql.Where("Archived=@0", false);
                if (filter.OwnerID != null)
                {
                    sql = sql.Append("and GCComUser.ID=@0", filter.OwnerID);
                }
                if (filter.CountryID != null)
                {
                    sql = sql.Append("and CountryID=@0", filter.CountryID);
                }
                if (!string.IsNullOrEmpty(filter.NameContainsWord))
                {
                    sql = sql.Append(string.Format("and GCComGeocache.Name like '%{0}%'", filter.NameContainsWord.Replace("'", "''")));
                }
                if (filter.OwnerName != null)
                {
                    sql = sql.Append(string.Format("and GCComUser.UserName like '%{0}%'", filter.OwnerName.Replace("'", "''")));
                }
                if (filter.CenterLat != null && filter.CenterLon != null && filter.Radius != null)
                {
                    sql.Append("and dbo.F_GREAT_CIRCLE_DISTANCE(GCComGeocache.Latitude, GCComGeocache.Longitude, @0, @1) < @2", filter.HomeLat, filter.HomeLon, filter.Radius);
                }
                if (minLat != null)
                {
                    sql = sql.Append("and GCComGeocache.Latitude>=@0", minLat);
                    sql = sql.Append("and GCComGeocache.Latitude<=@0", maxLat);
                    sql = sql.Append("and GCComGeocache.Longitude>=@0", minLon);
                    sql = sql.Append("and GCComGeocache.Longitude<=@0", maxLon);
                }
            }
            if (filter.MaxResult > 0 && filter.OrderBy != null && filter.OrderByDirection!=null)
            {
                int orderby = (int)filter.OrderBy;
                int orderbydir = (int)filter.OrderByDirection;
                string orderdir = orderbydir > 0 ? "ASC" : "DESC";
                switch (orderby)
                {
                    case (int)CacheListController.OrderOnItem.DistanceFromHome:
                        sql = sql.OrderBy(string.Format("dbo.F_GREAT_CIRCLE_DISTANCE(GCComGeocache.Latitude, GCComGeocache.Longitude, {0}, {1}) {2}", filter.HomeLat.ToString().Replace(',', '.'), filter.HomeLon.ToString().Replace(',', '.'), orderdir));
                        break;
                    case (int)CacheListController.OrderOnItem.HiddenDate:
                        sql = sql.OrderBy(string.Format("UTCPlaceDate {0}", orderdir));
                        break;
                }
            }
            return sql;
        }

        private GeocacheMapResult GetGeocaches(GeocacheSearchFilter filter, double minLat, double minLon, double maxLat, double maxLon, int zoomLevel)
        {
            //todo: depending on zoom level check found/hidden
            var result = new GeocacheMapResult();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                long callerGCComID = 0;
                string foundField = "FoundLogID = NULL";
                if (zoomLevel >= 12)
                {
                    var settings = _gcEuUserSettingsService.GetSettings();
                    if (settings != null && settings.GCComUserID != null)
                    {
                        callerGCComID = (long)settings.GCComUserID;
                        foundField = string.Format("FoundLogID = (select top 1 ID from GCComGeocacheLog  with (nolock) where FinderId={0} and GeocacheID=GCComGeocache.ID and WptLogTypeId in (2, 10, 11))", settings.GCComUserID);
                    }
                }

                var sql = PetaPoco.Sql.Builder;
                if (filter.MaxResult > 0)
                {
                    sql.Append(string.Format("select top {0} GCComGeocache.Latitude as a, GCComGeocache.Longitude as o, GCComGeocache.GeocacheTypeId as i, GCComGeocache.Code as c, GCComGeocache.OwnerId, {1}", filter.MaxResult, foundField));
                    addWhereClause(sql, filter, null, null, null, null);
                }
                else
                {
                    sql.Append(string.Format("select GCComGeocache.Latitude as a, GCComGeocache.Longitude as o, GCComGeocache.GeocacheTypeId as i, GCComGeocache.Code as c, GCComGeocache.OwnerId, {0}", foundField));
                    addWhereClause(sql, filter, minLat, minLon, maxLat, maxLon);
                }
                var items = db.Fetch<GeocacheMapItem>(sql);

                //personalize
                if (callerGCComID > 0)
                {
                    foreach (var item in items)
                    {
                        if (item.OwnerId == callerGCComID)
                        {
                            item.i = "o";
                        }
                        else if (item.FoundLogID != null)
                        {
                            item.i = "f";
                        }
                    }
                }

                result.Items = items.ToArray();
            }
            return result;
        }
    }
}
using Globalcaching.Core;
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
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Globalcaching.Controllers
{
    public class CacheMapController: Controller
    {
        public class GCLocation
        {
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
        }

        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IGeocacheSearchFilterService _geocacheSearchFilterService;
        private readonly ILiveAPIDownloadService _liveAPIDownloadService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public CacheMapController(IGCEuUserSettingsService gcEuUserSettingsService,
            IOrchardServices services,
            ILiveAPIDownloadService liveAPIDownloadService,
            IGeocacheSearchFilterService geocacheSearchFilterService,
            IWorkContextAccessor workContextAccessor)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _liveAPIDownloadService = liveAPIDownloadService;
            _geocacheSearchFilterService = geocacheSearchFilterService;
            _workContextAccessor = workContextAccessor;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index()
        {
            string gccode = HttpContext.Request.QueryString["wp"] as string;
            return ShowMap(new GeocacheSearchFilter(), gccode);
        }

        [Themed]
        public ActionResult MacroResult()
        {
            return ShowMap(new GeocacheSearchFilter() { MacroResult = true }, null);
        }

        [Themed]
        public ActionResult ShowMap(GeocacheSearchFilter filter, string geocacheCode)
        {
            CacheMapSettings mapSettings = new CacheMapSettings();
            mapSettings.CenterLat = 50.5;
            mapSettings.CenterLon = 5.5;
            mapSettings.InitialZoomLevel = 12;
            mapSettings.Filter = filter;

            var settings = _gcEuUserSettingsService.GetSettings();
            mapSettings.CanDownload = (settings != null && settings.YafUserID > 1 && !string.IsNullOrEmpty(settings.LiveAPIToken));
            if (filter.HomeLat == null)
            {
                if (settings != null)
                {
                    filter.HomeLat = settings.HomelocationLat;
                    filter.HomeLon = settings.HomelocationLon;
                }
            }

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                if (!string.IsNullOrEmpty(geocacheCode))
                {
                    var gc = db.FirstOrDefault<GCLocation>("select Latitude, Longitude from GCComGeocache where Code=@0", geocacheCode);
                    if (gc != null)
                    {
                        mapSettings.CenterLat = (double)gc.Latitude;
                        mapSettings.CenterLon = (double)gc.Longitude;
                        mapSettings.InitialZoomLevel = 20;
                    }
                }
                else if (filter.CenterLat != null && filter.CenterLon != null)
                {
                    mapSettings.CenterLat = (double)filter.CenterLat;
                    mapSettings.CenterLon = (double)filter.CenterLon;
                }
                else
                {
                    var sql = PetaPoco.Sql.Builder;
                    if (filter.MaxResult > 0)
                    {
                        sql.Append(string.Format("select top {0} GCComGeocache.Latitude as a, GCComGeocache.Longitude as o", filter.MaxResult));
                        _geocacheSearchFilterService.AddWhereClause(sql, filter);
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
                        _geocacheSearchFilterService.AddWhereClause(sql, filter);
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
            public string Url { get; set; }
            public string Name { get; set; }
            public DateTime UTCPlaceDate { get; set; }
            public long ContainerTypeId { get; set; }
            public double Difficulty { get; set; }
            public double Terrain { get; set; }
            public int? FavoritePoints { get; set; }

            public string Municipality { get; set; }
            public double? Distance { get; set; }

            public string UserName { get; set; }
            public Guid PublicGuid { get; set; }
        }

        [HttpPost]
        public ActionResult GetLatLonCoords(string id)
        {
            LatLon ll = LatLon.FromString(id);
            if (ll == null)
            {
                ll = Helper.GetLocationOfCity(id);
            }
            if (ll != null)
            {
                GeocacheMapInfo gmi = new GeocacheMapInfo();
                gmi.a = ll.lat;
                gmi.o = ll.lon;
                return Json(gmi);
            }
            return null;
        }

        [HttpPost]
        public ActionResult GetLocationCoords(string id)
        {
            string result = id;
            LatLon ll = LatLon.FromString(id);
            if (ll == null)
            {
                ll = Helper.GetLocationOfCity(id);
            }
            if (ll != null)
            {
                return Content(Helper.GetCoordinatesPresentation(ll.lat, ll.lon));
            }
            return null;
        }

        [HttpPost]
        public ActionResult GetAreaInfo(string id)
        {
            string result = "";
            LatLon ll = LatLon.FromString(id);
            if (ll != null)
            {
                List<Globalcaching.Core.SHP.AreaInfo> ais = Globalcaching.Core.SHP.ShapeFilesManager.Instance.GetAreasOfLocation(ll, Globalcaching.Core.SHP.ShapeFilesManager.Instance.GetAreasByLevel(Globalcaching.Core.SHP.AreaType.Other));
                StringBuilder sb = new StringBuilder();
                foreach (Globalcaching.Core.SHP.AreaInfo ai in ais)
                {
                    if (!string.IsNullOrEmpty(ai.Owner.EMail))
                    {
                        sb.AppendLine(string.Format("Gebied: {0}<br />", HttpUtility.HtmlEncode(ai.Name)));
                        sb.AppendLine(string.Format("Beheerder: {0}<br />", HttpUtility.HtmlEncode(ai.Owner.Name)));
                        sb.AppendLine(string.Format("Website: <a href=\"{0}\" target=\"_blank\">{0}</a><br />", HttpUtility.HtmlEncode(ai.Owner.Website)));
                        List<string> extraInfo = Globalcaching.Core.SHP.ShapeFilesManager.Instance.GetAdditionalInfoOfArea(ai, ll);
                        sb.AppendLine("Additionele informatie:<br />");
                        foreach (string s in extraInfo)
                        {
                            sb.AppendLine(string.Format("{0}<br />", HttpUtility.HtmlEncode(s)));
                        }
                    }
                }
                result = sb.ToString();
            }
            return Content(result);
        }

        [HttpPost]
        public ActionResult GetAreaPolygons(string minlatminlon, string maxlatmaxlon, string reset)
        {
            StringBuilder result = new StringBuilder();

            List<object> processedIds;
            processedIds = (List<object>)HttpContext.Session["AREAINFO_IDS"];
            if (processedIds == null)
            {
                processedIds = new List<object>();
                HttpContext.Session["AREAINFO_IDS"] = processedIds;
            }
            if (reset == "1")
            {
                processedIds.Clear();
            }
            string[] parts = minlatminlon.Split(new char[] { ' ', ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            double minlat = Helper.ConvertToDouble(parts[0]);
            double minlon = Helper.ConvertToDouble(parts[1]);
            parts = maxlatmaxlon.Split(new char[] { ' ', ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            double maxlat = Helper.ConvertToDouble(parts[0]);
            double maxlon = Helper.ConvertToDouble(parts[1]);

            //get areasinfo that is visible
            Globalcaching.Core.SHP.ShapeFilesManager sm = Globalcaching.Core.SHP.ShapeFilesManager.Instance;
            List<Globalcaching.Core.SHP.AreaInfo> allais = sm.GetAreasByLevel(Globalcaching.Core.SHP.AreaType.Other);
            List<Globalcaching.Core.SHP.AreaInfo> aiInView = (from ai in allais where !string.IsNullOrEmpty(ai.Owner.EMail) && maxlat > ai.MinLat && minlat < ai.MaxLat && maxlon > ai.MinLon && minlon < ai.MaxLon select ai).ToList();
            bool first = true;
            result.Append("[");
            foreach (Globalcaching.Core.SHP.AreaInfo ai in aiInView)
            {
                List<Globalcaching.Core.SHP.ShapeFile.IndexRecord> recs = (from r in ai.Owner.IndexRecords where !processedIds.Contains(r.ID) && r.Name == ai.Name && maxlat > r.YMin && minlat < r.YMax && maxlon > r.XMin && minlon < r.XMax select r).ToList();
                foreach (Globalcaching.Core.SHP.ShapeFile.IndexRecord rec in recs)
                {
                    List<LatLonPolygon> polys = sm.GetPolygonOfArea(rec);
                    foreach (LatLonPolygon poly in polys)
                    {
                        if (!first) result.Append(",");
                        first = false;
                        result.AppendFormat("{{'id': '{0}', 'points': [ ", rec.ID.ToString().Replace('\'', ' '));
                        bool firstpoint = true;
                        foreach (LatLon ll in poly.points)
                        {
                            if (!firstpoint) result.Append(",");
                            firstpoint = false;
                            result.AppendFormat("{{'lat': {0}, 'lon': {1}}} ", ll.lat.ToString("0.0000").Replace(',', '.'), ll.lon.ToString("0.0000").Replace(',', '.'));
                        }
                        processedIds.Add(rec.ID);
                        result.Append("]}");
                    }

                    if (result.ToString().Length > 50000)
                    {
                        break;
                    }
                }
            }
            result.Append("]");
            return Content(result.ToString());
        }

        [HttpPost]
        public ActionResult GetWaypointInfo(string code)
        {
            WaypointInfo wpi = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder
                    .Select("GCComGeocache.Code"
                    , "GCComGeocache.GeocacheTypeId"
                    , "GCComGeocache.Url"
                    , "GCComGeocache.Name"
                    , "GCComGeocache.UTCPlaceDate"
                    , "GCComGeocache.ContainerTypeId"
                    , "GCComGeocache.Difficulty"
                    , "GCComGeocache.Terrain"
                    , "GCComGeocache.FavoritePoints"
                    , "GCEuGeocache.Municipality"
                    , "GCEuGeocache.Distance"
                    , "GCComUser.UserName"
                    , "GCComUser.PublicGuid"
                    )
                    .From("GCComGeocache")
                    .InnerJoin("GCEuData.dbo.GCEuGeocache").On("GCComGeocache.ID=GCEuGeocache.ID")
                    .InnerJoin("GCComUser").On("GCComGeocache.OwnerId=GCComUser.ID")
                    .Where("GCComGeocache.Code=@0", code);

                wpi = db.FirstOrDefault<WaypointInfo>(sql);
            }
            return Json(wpi);
        }

        [Themed]
        public ActionResult CopyViewToDownload(GeocacheSearchFilter filter, string minLat, string minLon, string maxLat, string maxLon)
        {
            bool result = false;

            var setting = _gcEuUserSettingsService.GetSettings();
            if (setting != null && setting.YafUserID > 1)
            {
                string top = "";
                if (filter.MaxResult > 0)
                {
                    top = string.Format("top {0}", filter.MaxResult);
                }
                var sql = PetaPoco.Sql.Builder
                    .Append(string.Format("insert into GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo (Code) select {1} GCComGeocache.Code", setting.YafUserID, top));
                sql = _geocacheSearchFilterService.AddWhereClause(sql, filter, Core.Helper.ConvertToDouble(minLat), Core.Helper.ConvertToDouble(minLon), Core.Helper.ConvertToDouble(maxLat), Core.Helper.ConvertToDouble(maxLon));
                result = _liveAPIDownloadService.SetQueryResultForDownload(sql);
            }
            if (result)
            {
                var m = _liveAPIDownloadService.DownloadStatus;
                if (m == null)
                {
                    return HttpNotFound("Fout bij het ophalen van de download status");
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("De geocaches kunnen worden gedownload."));
                    return View("../DisplayTemplates/Parts.LiveAPIDownload", m);
                }
            }
            else
            {
                var m = _liveAPIDownloadService.DownloadStatus;
                if (m == null)
                {
                    return HttpNotFound("Fout bij het ophalen van de download status");
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Er is een fout opgetreden. Je moet ingelogd zijn en controlleer of er al geocaches gedownload worden."));
                    //return View("~/Views/DisplayTemplates/Parts.LiveAPIDownload", m);
                    return View("../DisplayTemplates/Parts.LiveAPIDownload", m);
                }
            }
        }

        [HttpPost]
        public ActionResult SearchGeocaches(GeocacheSearchFilter filter, double minLat, double minLon, double maxLat, double maxLon, int zoom)
        {
            return Json(GetGeocaches(filter, minLat, minLon, maxLat, maxLon, zoom));
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
                    _geocacheSearchFilterService.AddWhereClause(sql, filter);
                }
                else
                {
                    sql.Append(string.Format("select GCComGeocache.Latitude as a, GCComGeocache.Longitude as o, GCComGeocache.GeocacheTypeId as i, GCComGeocache.Code as c, GCComGeocache.OwnerId, {0}", foundField));
                    _geocacheSearchFilterService.AddWhereClause(sql, filter, minLat, minLon, maxLat, maxLon);
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
using Gavaghan.Geodesy;
using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class CacheListController: Controller
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly ILiveAPIDownloadService _liveAPIDownloadService;
        private readonly IGeocacheSearchFilterService _geocacheSearchFilterService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public CacheListController(IGCEuUserSettingsService gcEuUserSettingsService,
            ILiveAPIDownloadService liveAPIDownloadService,
            IGeocacheSearchFilterService geocacheSearchFilterService,
            IOrchardServices services)
        {
            _liveAPIDownloadService = liveAPIDownloadService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _geocacheSearchFilterService = geocacheSearchFilterService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index()
        {
            return Search(new GeocacheSearchFilter());
        }

        [Themed]
        public ActionResult Parels()
        {
            return Search(new GeocacheSearchFilter() { Parel = true });
        }

        [Themed]
        public ActionResult MostRecentCountry(int id)
        {
            return Search(new GeocacheSearchFilter() { CountryID = id });
        }

        [Themed]
        public ActionResult NameSeries(int countryId, string nameSeriesMatch)
        {
            return Search(new GeocacheSearchFilter() { CountryID = countryId, NameSeriesMatch = nameSeriesMatch });
        }

        [Themed]
        public ActionResult FromOwner(int id)
        {
            return Search(new GeocacheSearchFilter() { OwnerID = id });
        }

        [Themed]
        public ActionResult MacroResult()
        {
            return Search(new GeocacheSearchFilter() { MacroResult = true });
        }

        [Themed]
        public ActionResult QuickSearch(GCComQuickGeocacheSearch qsFilter)
        {
            var filter = new GeocacheSearchFilter();
            filter.OrderBy = (int)GeocacheSearchFilterOrderOnItem.DistanceFromHome;
            filter.OrderByDirection = 1;
            if (qsFilter.CountryID > 0)
            {
                filter.CountryID = qsFilter.CountryID;
            }
            if (!string.IsNullOrEmpty(qsFilter.NameContainsWord))
            {
                filter.NameContainsWord = qsFilter.NameContainsWord;
            }
            if (!string.IsNullOrEmpty(qsFilter.HiddenBy))
            {
                filter.OwnerName = qsFilter.HiddenBy;
            }
            if (!string.IsNullOrEmpty(qsFilter.Location))
            {
                double r = 0;
                if (double.TryParse(qsFilter.Radius, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out r))
                {
                    filter.Radius = r;
                    LatLon ll = LatLon.FromString(qsFilter.Location);
                    if (ll == null)
                    {
                        ll = Helper.GetLocationOfCity(qsFilter.Location);
                    }
                    if (ll != null)
                    {
                        filter.CenterLat = ll.lat;
                        filter.CenterLon = ll.lon;
                        filter.HomeLat = ll.lat;
                        filter.HomeLon = ll.lon;
                    }
                    else
                    {
                        Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("De opgegeven locatie is niet geldig."));
                    }
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("De opgegeven radius is niet geldig."));
                }
            }
            return Search(filter);
        }

        [Themed]
        public ActionResult Search(GeocacheSearchFilter filter)
        {
            filter.Page = 1;
            filter.PageSize = 20;
            if (filter.HomeLat == null || filter.HomeLon == null)
            {
                if (filter.CenterLat != null && filter.CenterLon != null)
                {
                    filter.HomeLat = filter.CenterLat;
                    filter.HomeLon = filter.CenterLon;
                }
                else
                {
                    var setting = _gcEuUserSettingsService.GetSettings();
                    if (setting != null && setting.HomelocationLat != null && setting.HomelocationLon!=null)
                    {
                        filter.HomeLat = setting.HomelocationLat;
                        filter.HomeLon = setting.HomelocationLon;
                    }
                }
            }
            return View("Search", GetGeocaches(filter));
        }

        [HttpPost]
        [Themed]
        public ActionResult UpdateSortingGeocaches(GeocacheSearchFilter filter, int newSortBy, int newSortDir)
        {
            var setting = _gcEuUserSettingsService.GetSettings();
            if (setting != null)
            {
                setting.SortGeocachesBy = newSortBy;
                setting.SortGeocachesDirection = newSortDir;
                _gcEuUserSettingsService.UpdateSettings(setting);
            }
            filter.OrderBy = newSortBy;
            filter.OrderByDirection = newSortDir;
            return Search(filter);
        }


        [Themed]
        public ActionResult CopyListToDownload(GeocacheSearchFilter filter)
        {
            bool result = false;
            if (filter.MacroResult == true)
            {
                result = _liveAPIDownloadService.SetMacroResultForDownload();
            }
            else
            {
                var setting = _gcEuUserSettingsService.GetSettings();
                if (setting != null && setting.YafUserID > 1)
                {
                    var sql = PetaPoco.Sql.Builder
                        .Append(string.Format("insert into GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo (Code) select GCComGeocache.Code", setting.YafUserID));
                    sql = _geocacheSearchFilterService.AddWhereClause(sql, filter);
                    result = _liveAPIDownloadService.SetQueryResultForDownload(sql);
                }
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
        public ActionResult SearchGeocaches(GeocacheSearchFilter filter)
        {
            return Json(GetGeocaches(filter));
        }


        private GeocacheSearchResult GetGeocaches(GeocacheSearchFilter filter)
        {
            GeocacheSearchResult result = new GeocacheSearchResult();
            result.Filter = filter;
            result.PageCount = 1;
            result.CurrentPage = 1;

            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.SortGeocachesBy!=null && settings.SortGeocachesDirection!=null)
            {
                filter.OrderBy = settings.SortGeocachesBy;
                filter.OrderByDirection = settings.SortGeocachesDirection;
            }

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                string euDatabase = Core.Helper.GetTableNameFromConnectionString(dbGcEuDataConnString);
                var sql = PetaPoco.Sql.Builder
                    .Select("GCComGeocache.ID", "GCComGeocache.Code", "GCComGeocache.Archived", "GCComGeocache.Available", "GCComGeocache.Latitude", "GCComGeocache.Longitude", "GeocacheTypeId", "Difficulty", "Terrain", "ContainerTypeId", "Municipality", "OwnerId", "UTCPlaceDate", "Country", "Name", "Url", "GCEuGeocache.City", "GCComUser.PublicGuid", "GCComUser.UserName", "FavoritePoints", "GCEuGeocache.FoundCount", "GCEuGeocache.MostRecentFoundDate", "GCEuGeocache.PublishedAtDate", "GCEuGeocache.Distance");
                if (filter.HomeLat!=null && filter.HomeLon!=null)
                {
                    sql.Append(",dbo.F_GREAT_CIRCLE_DISTANCE(GCComGeocache.Latitude, GCComGeocache.Longitude, @0, @1) AS DistanceFromHome", filter.HomeLat, filter.HomeLon);
                }
                else
                {
                    sql.Append(",DistanceFromHome=NULL", filter.HomeLat, filter.HomeLon);
                }
                sql = _geocacheSearchFilterService.AddWhereClause(sql, filter, true);
                if (filter.MaxResult > 0)
                {
                    filter.Page = 1;
                    filter.PageSize = filter.MaxResult;
                }

                var items = db.Page<GeocacheListItem>(filter.Page, filter.PageSize, sql);
                result.Items = items.Items.ToArray();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;

                if (filter.MaxResult > 0)
                {
                    result.PageCount = 1;
                    result.TotalCount = items.TotalItems;
                    if (result.TotalCount > filter.MaxResult)
                    {
                        result.TotalCount = filter.MaxResult;
                    }
                }

                foreach(var item in result.Items)
                {
                    if (filter.HomeLat!=null && filter.HomeLon!=null && item.Latitude!=null && item.Longitude!=null)
                    {
                        GeodeticMeasurement gm = Helper.CalculateDistance((double)filter.HomeLat, (double)filter.HomeLon, (double)item.Latitude, (double)item.Longitude);
                        item.DirectionIcon = Helper.GetWindDirection(gm.Azimuth);

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
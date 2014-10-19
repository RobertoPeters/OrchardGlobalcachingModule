﻿using Gavaghan.Geodesy;
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
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public enum OrderOnItem: int
        {
            DistanceFromHome = 0,
            HiddenDate = 1
        }

        public CacheListController(IGCEuUserSettingsService gcEuUserSettingsService,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index()
        {
            return Search(new GeocacheSearchFilter() { OrderBy = (int)OrderOnItem.HiddenDate, OrderByDirection=-1 });
        }

        [Themed]
        public ActionResult MostRecentCountry(int id)
        {
            return Search(new GeocacheSearchFilter() { OrderBy = (int)OrderOnItem.HiddenDate, OrderByDirection = -1, CountryID=id });
        }

        [Themed]
        public ActionResult FromOwner(int id)
        {
            return Search(new GeocacheSearchFilter() { OrderBy = (int)OrderOnItem.HiddenDate, OrderByDirection = -1,  OwnerID = id });
        }

        [Themed]
        public ActionResult MacroResult()
        {
            return Search(new GeocacheSearchFilter() { OrderBy = (int)OrderOnItem.HiddenDate, OrderByDirection = -1, MacroResult=true });
        }

        [Themed]
        public ActionResult QuickSearch(GCComQuickGeocacheSearch qsFilter)
        {
            var filter = new GeocacheSearchFilter();
            filter.OrderBy = (int)OrderOnItem.DistanceFromHome;
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

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                string euDatabase = Core.Helper.GetTableNameFromConnectionString(dbGcEuDataConnString);
                var sql = PetaPoco.Sql.Builder
                    .Select("GCComGeocache.ID", "GCComGeocache.Code", "GCComGeocache.Latitude", "GCComGeocache.Longitude", "GeocacheTypeId", "Difficulty", "Terrain", "ContainerTypeId", "Municipality", "OwnerId", "UTCPlaceDate", "Country", "Name", "Url", "GCEuGeocache.City", "GCComUser.PublicGuid", "GCComUser.UserName", "FavoritePoints", "GCEuGeocache.FoundCount");
                if (filter.HomeLat!=null && filter.HomeLon!=null)
                {
                    sql.Append(",dbo.F_GREAT_CIRCLE_DISTANCE(GCComGeocache.Latitude, GCComGeocache.Longitude, @0, @1) AS DistanceFromHome", filter.HomeLat, filter.HomeLon);
                }
                else
                {
                    sql.Append(",DistanceFromHome=NULL", filter.HomeLat, filter.HomeLon);
                }
                sql = sql.From("GCComGeocache")
                    .InnerJoin(string.Format("[{0}].[dbo].[GCEuGeocache]", euDatabase)).On("GCComGeocache.ID = GCEuGeocache.ID")
                    .InnerJoin(string.Format("GCComUser", euDatabase)).On("GCComGeocache.OwnerId = GCComUser.ID");

                if (filter.MacroResult != null)
                {
                    sql = sql.InnerJoin(string.Format("GCEuMacroData.dbo.macro_{0}_Resultaat", settings.YafUserID)).On(string.Format("GCComGeocache.ID = GCEuMacroData.dbo.macro_{0}_Resultaat.ID", settings.YafUserID));
                }
                else
                {
                    //add filters
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
                }

                int orderby = filter.OrderBy ?? (int)OrderOnItem.DistanceFromHome;
                int orderbydir = filter.OrderByDirection ?? 1;
                if (orderby == (int)OrderOnItem.DistanceFromHome && (filter.HomeLat == null || filter.HomeLon == null))
                {
                    orderby = (int)OrderOnItem.HiddenDate;
                    orderbydir = -1;
                }
                string orderdir = orderbydir > 0 ? "ASC" : "DESC";
                switch (orderby)
                {
                    case (int)OrderOnItem.DistanceFromHome:
                        sql = sql.OrderBy(string.Format("dbo.F_GREAT_CIRCLE_DISTANCE(GCComGeocache.Latitude, GCComGeocache.Longitude, {0}, {1}) {2}", filter.HomeLat.ToString().Replace(',', '.'), filter.HomeLon.ToString().Replace(',', '.'), orderdir));
                        break;
                    case (int)OrderOnItem.HiddenDate:
                        sql = sql.OrderBy(string.Format("UTCPlaceDate {0}", orderdir));
                        break;
                }

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
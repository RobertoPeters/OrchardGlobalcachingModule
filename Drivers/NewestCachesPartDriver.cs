using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class NewestCachesPartDriver : ContentPartDriver<NewestCachesPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;
        private readonly IGeocacheSearchFilterService _geocacheSearchFilterService;

        public NewestCachesPartDriver(IGCEuUserSettingsService userSettingsService, 
            IGeocacheSearchFilterService geocacheSearchFilterService)
        {
            _userSettingsService = userSettingsService;
            _geocacheSearchFilterService = geocacheSearchFilterService;
        }

        protected override DriverResult Display(NewestCachesPart part, string displayType, dynamic shapeHelper)
        {
            /*
             * modes:
             * 0 - netherlands
             * 1 - belgium
             * 2 - luxembourg
             * 3 - radius from home
             * 4 - radius from home ftf open
             */
            NewestCachesModel m = new NewestCachesModel();
            m.Mode = 0;
            GeocacheSearchFilter filter = new GeocacheSearchFilter();
            filter.OrderBy = (int)GeocacheSearchFilterOrderOnItem.PublicationDate;
            filter.OrderByDirection = -1;
            filter.MaxResult = 10;
            filter.CountryID = 141;
            var settings = _userSettingsService.GetSettings();
            if (settings != null)
            {
                m.Mode = settings.NewestCachesMode ?? 0;
                filter.CountryID = settings.DefaultCountryCode ?? 141;
                if (settings.HomelocationLat != null && settings.HomelocationLon != null)
                {
                    filter.HomeLat = settings.HomelocationLat;
                    filter.HomeLon = settings.HomelocationLon;
                }
                else if (m.Mode == 3 || m.Mode == 4)
                {
                    m.Mode = 0;
                    settings.NewestCachesMode = null;
                }

                if (settings.NewestCachesMode == null && settings.DefaultCountryCode != null)
                {
                    switch (filter.CountryID)
                    {
                        case 141:
                            m.Mode = 0;
                            break;
                        case 4:
                            m.Mode = 1;
                            break;
                        case 8:
                            m.Mode = 2;
                            break;
                    }
                }
            }
            switch (m.Mode)
            {
                case 0:
                    filter.CountryID = 141;
                    break;
                case 1:
                    filter.CountryID = 4;
                    break;
                case 2:
                    filter.CountryID = 8;
                    break;
                case 3:
                    filter.CenterLat = settings.HomelocationLat;
                    filter.CenterLon = settings.HomelocationLon;
                    filter.Radius = 30;
                    filter.CountryID = null;
                    break;
                case 4:
                    filter.CenterLat = settings.HomelocationLat;
                    filter.CenterLon = settings.HomelocationLon;
                    filter.Radius = 30;
                    filter.FTFOpen = true;
                    filter.CountryID = null;
                    break;
            }
            m.Geocaches = _geocacheSearchFilterService.GetGeocaches(filter);
            return ContentShape("Parts_NewestCaches",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.NewestCaches",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
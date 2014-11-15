using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class EditUserSettingsPartDriver : ContentPartDriver<GCEuUserSettingsPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        protected override string Prefix { get { return ""; } }

        public EditUserSettingsPartDriver(IGCEuUserSettingsService userSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _userSettingsService = userSettingsService;
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        protected override DriverResult Display(GCEuUserSettingsPart part, string displayType, dynamic shapeHelper)
        {
            var curSettings = _userSettingsService.GetSettings();
            var settings = new EditUserSettingsModel();
            if (curSettings != null)
            {
                settings.DefaultCountryCode = curSettings.DefaultCountryCode ?? 141;
                settings.ShowGeocachesOnGlobal = curSettings.ShowGeocachesOnGlobal ?? true;
                settings.MarkLogTextColor1 = curSettings.MarkLogTextColor1 ?? "";
                settings.MarkLogTextColor2 = curSettings.MarkLogTextColor2 ?? "";
                settings.MarkLogTextColor3 = curSettings.MarkLogTextColor3 ?? "";
                if (curSettings.HomelocationLat != null && curSettings.HomelocationLon != null)
                {
                    settings.Homelocation = Core.Helper.GetCoordinatesPresentation((double)curSettings.HomelocationLat, (double)curSettings.HomelocationLon);
                }
                else
                {
                    settings.Homelocation = "";
                }
                settings.ReturnUrl = HttpContext.Request.Url.ToString();
            }
            else
            {
            }

            return ContentShape("Parts_EditUserSettings",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.EditUserSettings",
                            Model: settings,
                            Prefix: Prefix));
        }
    }
}
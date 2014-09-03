using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GCComQuickGeocacheSearchPartDriver : ContentPartDriver<GCComQuickGeocacheSearchPart>
    {
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        protected override string Prefix { get { return ""; } }

        public GCComQuickGeocacheSearchPartDriver(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(GCComQuickGeocacheSearchPart part, string displayType, dynamic shapeHelper)
        {
            GCComQuickGeocacheSearch model = new GCComQuickGeocacheSearch();
            model.Settings = _gcEuUserSettingsService.GetSettings();

            return ContentShape("Parts_GCComQuickGeocacheSearch",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GCComQuickGeocacheSearch",
                            Model: model,
                            Prefix: Prefix));
        }
    }
}
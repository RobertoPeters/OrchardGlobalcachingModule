using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GCComSearchGeocacheLogsOfUserPartDriver : ContentPartDriver<GCComSearchLogsPart>
    {
        private readonly IGCComSearchGeocacheLogsService _gcComSearchGeocacheLogsService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        protected override string Prefix { get { return "Globalcaching.GCComSearchLogsPart"; } }

        public GCComSearchGeocacheLogsOfUserPartDriver(IGCComSearchGeocacheLogsService gcComSearchGeocacheLogsService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcComSearchGeocacheLogsService = gcComSearchGeocacheLogsService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(GCComSearchLogsPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return ContentShape("Parts_GCComSearchGeocacheLogsUser",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.GCComSearchGeocacheLogsUser",
                                Model: null,
                                Prefix: Prefix));
            }
            else
            {
                return ContentShape("Parts_ForDonatorsOnly",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.ForDonatorsOnly",
                                Model: null,
                                Prefix: Prefix));
            }
        }
    }
}
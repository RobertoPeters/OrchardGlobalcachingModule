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
    public class GeocacheMaintenancePartDriver : ContentPartDriver<GeocacheMaintenancePart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IGeocacheMaintenanceService _geocacheMaintenanceService;

        public GeocacheMaintenancePartDriver(IGCEuUserSettingsService gcEuUserSettingsService,
            IGeocacheMaintenanceService geocacheMaintenanceService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _geocacheMaintenanceService = geocacheMaintenanceService;
        }

        protected override DriverResult Display(GeocacheMaintenancePart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return ContentShape("Parts_GeocacheMaintenance",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.GeocacheMaintenance",
                                Model: _geocacheMaintenanceService.GetGeocacheMaintenanceInfo(settings.GCComUserID),
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
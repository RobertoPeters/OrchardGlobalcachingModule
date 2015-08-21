using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GeocacheBatchLogPartDriver : ContentPartDriver<GeocacheBatchLogPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;

        public GeocacheBatchLogPartDriver(IGCEuUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        protected override DriverResult Display(GeocacheBatchLogPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                if (settings.YafUserID > 1 && !string.IsNullOrEmpty(settings.LiveAPIToken))
                {
                    return ContentShape("Parts_GeocacheBatchLog",
                            () => shapeHelper.DisplayTemplate(
                                    TemplateName: "Parts.GeocacheBatchLog",
                                    Model: null,
                                    Prefix: Prefix));
                }
                else
                {
                    return null;
                }
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
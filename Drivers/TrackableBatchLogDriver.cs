using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class TrackableBatchLogDriver : ContentPartDriver<TrackableBatchLogPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;

        public TrackableBatchLogDriver(IGCEuUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        protected override DriverResult Display(TrackableBatchLogPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return ContentShape("Parts_TrackableBatchLog",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.TrackableBatchLog",
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
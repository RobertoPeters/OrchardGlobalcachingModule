using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GCComSearchUserPartDriver : ContentPartDriver<GCComSearchUserPart>
    {
        private readonly IGCComSearchUserService _gcComSearchUserService;

        protected override string Prefix { get { return "Globalcaching.GCComSearchUserPart"; } }
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GCComSearchUserPartDriver(IGCComSearchUserService gcComSearchUserService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcComSearchUserService = gcComSearchUserService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(GCComSearchUserPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return ContentShape("Parts_GCComSearchUser",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.GCComSearchUser",
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
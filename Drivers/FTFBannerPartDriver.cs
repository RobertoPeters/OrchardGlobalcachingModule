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
    public class FTFBannerPartDriver : ContentPartDriver<FTFBannerPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IGCEuUserSettingsService _userSettingsService;

        public FTFBannerPartDriver(IGCEuUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        protected override DriverResult Display(FTFBannerPart part, string displayType, dynamic shapeHelper)
        {
            var m = _userSettingsService.GetSettings();
            if (m != null && m.IsDonator)
            {
                return ContentShape("Parts_FTFBanner",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.FTFBanner",
                                Model: m,
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
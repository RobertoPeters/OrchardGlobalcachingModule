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
    public class CombiBannerPartDriver : ContentPartDriver<CombiBannerPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IGCEuUserSettingsService _userSettingsService;

        public CombiBannerPartDriver(IGCEuUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        protected override DriverResult Display(CombiBannerPart part, string displayType, dynamic shapeHelper)
        {
            var m = _userSettingsService.GetSettings();
            return ContentShape("Parts_CombiBanner",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.CombiBanner",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
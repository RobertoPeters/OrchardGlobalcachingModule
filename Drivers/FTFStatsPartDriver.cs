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
    public class FTFStatsPartPartDriver : ContentPartDriver<FTFStatsPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IFTFStatsService _ftfStatsService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public FTFStatsPartPartDriver(IFTFStatsService ftfStatsService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _ftfStatsService = ftfStatsService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(FTFStatsPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                var m = _ftfStatsService.GetFTFRanking(null, DateTime.Now.Year, 0, 1, 50);
                return ContentShape("Parts_FTFStats",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.FTFStats",
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
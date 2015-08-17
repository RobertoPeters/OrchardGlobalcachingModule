using Globalcaching.Models;
using Globalcaching.Services;
using Orchard;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class FoundsPerCountryRankingPartDriver : ContentPartDriver<FoundsPerCountryRankingPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;
        private readonly IFoundsPerCountryRankingService _foundsPerCountryRankingService;

        public FoundsPerCountryRankingPartDriver(IGCEuUserSettingsService userSettingsService, 
            IFoundsPerCountryRankingService foundsPerCountryRankingService)
        {
            _userSettingsService = userSettingsService;
            _foundsPerCountryRankingService = foundsPerCountryRankingService;
        }

        protected override DriverResult Display(FoundsPerCountryRankingPart part, string displayType, dynamic shapeHelper)
        {
            int cntryId = 141;
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                if (settings.DefaultCountryCode != null)
                {
                    cntryId = (int)settings.DefaultCountryCode;
                }
                var m = _foundsPerCountryRankingService.GetRanking(1, 20, DateTime.Now.Year, cntryId, null);
                return ContentShape("Parts_FoundsPerCountryRanking",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.FoundsPerCountryRanking",
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
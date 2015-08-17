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
    public class CombiRankingPartDriver : ContentPartDriver<CombiRankingPart>
    {
        private readonly ICombiRankingService _combiRankingService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public CombiRankingPartDriver(ICombiRankingService combiRankingService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _combiRankingService = combiRankingService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(CombiRankingPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                var m = _combiRankingService.GetRanking(1, 50, 0, DateTime.Now.Year, null);
                return ContentShape("Parts_CombiRanking",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.CombiRanking",
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
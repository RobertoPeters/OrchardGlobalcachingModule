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
    public class LogImageStatsPartPartDriver : ContentPartDriver<LogImageStatsPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly ILogImageStatsService _logImageStatsService;

        public LogImageStatsPartPartDriver(ILogImageStatsService logImageStatsService)
        {
            _logImageStatsService = logImageStatsService;
        }

        protected override DriverResult Display(LogImageStatsPart part, string displayType, dynamic shapeHelper)
        {
            LogImageStatsFilter filter = new LogImageStatsFilter();
            filter.CountryId = 141;
            filter.MinDaysOnline = 0;
            filter.MinImageCount = 5;
            filter.MinFoundCount = 1;
            filter.CacheTypeId = 3;
            filter.SortOn = 0;
            var m = _logImageStatsService.GetLogImageStats(1, 25, filter);
            return ContentShape("Parts_LogImageStats",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.LogImageStats",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
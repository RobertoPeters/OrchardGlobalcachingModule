using Globalcaching.Services;
using Globalcaching.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class LogImageStatsController: Controller
    {
        private readonly ILogImageStatsService _logImageStatsService;

        public LogImageStatsController(ILogImageStatsService logImageStatsService)
        {
            _logImageStatsService = logImageStatsService;
        }

        public ActionResult GetLogImageStats(int page, int pageSize, int countryId, int cacheType, int minImgs, int minFounds, int minDays, int sortOn)
        {
            var filter = new LogImageStatsFilter();
            filter.CountryId = countryId;
            filter.MinDaysOnline = minDays;
            filter.MinImageCount = minImgs;
            filter.MinFoundCount = minFounds;
            filter.CacheTypeId = cacheType;
            filter.SortOn = sortOn;
            return Json(_logImageStatsService.GetLogImageStats(page, pageSize, filter));
        }
    }
}
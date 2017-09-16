using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class FoundsPerCountryRankingController: Controller
    {
        private readonly IFoundsPerCountryRankingService _foundsPerCountryRankingService;

        public FoundsPerCountryRankingController(IFoundsPerCountryRankingService foundsPerCountryRankingService)
        {
            _foundsPerCountryRankingService = foundsPerCountryRankingService;
        }

        [HttpPost]
        public ActionResult GetFoundsRanking(int page, int pageSize, int rankyear, int countryid, string nameFilter)
        {
            return Json(_foundsPerCountryRankingService.GetRanking(page, pageSize, rankyear, countryid, nameFilter));
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Banner(long id, int countryid, int year, int type)
        {
            _foundsPerCountryRankingService.CreateBanner(Response, id, countryid, year, type);
            return null;
        }

    }
}
using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class CombiRankingController: Controller
    {
        private readonly ICombiRankingService _combiRankingService;

        public CombiRankingController(ICombiRankingService combiRankingService)
        {
            _combiRankingService = combiRankingService;
        }

        [HttpPost]
        public ActionResult GetCombiRanking(int page, int pageSize, int rankType, int rankyear, string nameFilter)
        {
            return Json(_combiRankingService.GetRanking(page, pageSize, rankType, rankyear, nameFilter));
        }
    }
}
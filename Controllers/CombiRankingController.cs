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

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Banner()
        {
            string sid = Request.QueryString["id"];
            string syear = Request.QueryString["year"];
            string stype = Request.QueryString["type"];

            _combiRankingService.CreateCombiBanner(Response, sid, syear, stype);
            return null;
        }

        [HttpPost]
        public ActionResult GetCombiRanking(int page, int pageSize, int rankType, int rankyear, string nameFilter)
        {
            return Json(_combiRankingService.GetRanking(page, pageSize, rankType, rankyear, nameFilter));
        }
    }
}
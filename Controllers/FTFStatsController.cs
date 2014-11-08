using Globalcaching.Services;
using Globalcaching.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class FTFStatsController: Controller
    {
        private readonly IFTFStatsService _ftfStatsService;

        public FTFStatsController(IFTFStatsService ftfStatsService)
        {
            _ftfStatsService = ftfStatsService;
        }

        public ActionResult GetFTFStats(int page, int pageSize, string UserName, int Jaar, int RankingType)
        {
            return Json(_ftfStatsService.GetFTFRanking(UserName, Jaar, RankingType, page, pageSize));
        }
    }
}
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Themes;
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
        private readonly ITaskSchedulerService _taskSchedulerService;
        public IOrchardServices Services { get; set; }

        public FTFStatsController(IFTFStatsService ftfStatsService,
            ITaskSchedulerService taskSchedulerService,
            IOrchardServices services)
        {
            Services = services;
            _ftfStatsService = ftfStatsService;
            _taskSchedulerService = taskSchedulerService;
        }

        public ActionResult GetFTFStats(int page, int pageSize, string UserName, int Jaar, int RankingType)
        {
            return Json(_ftfStatsService.GetFTFRanking(UserName, Jaar, RankingType, page, pageSize));
        }

        [Themed]
        public ActionResult AddGeocache(string gccode)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                if (!string.IsNullOrEmpty(gccode) && gccode.ToUpper().StartsWith("GC"))
                {
                    List<string> l = new List<string>();
                    l.Add(gccode.ToUpper().Trim());
                    _taskSchedulerService.AddScheduledWaypoint(l, true);
                }
                var m = _ftfStatsService.GetUnassignedFTF(1, 50);
                m.QueueLength = _taskSchedulerService.GetScheduledGeocaches().Count();
                return View("Home", m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult Banner()
        {
            string sid = Request.QueryString["id"];
            string syear = Request.QueryString["year"];
            string stype = Request.QueryString["type"];

            _ftfStatsService.CreateFTFBanner(Response, sid, syear, stype);
            return null;
        }

        [Themed]
        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                var m = _ftfStatsService.GetUnassignedFTF(1, 50);
                m.QueueLength = _taskSchedulerService.GetScheduledGeocaches().Count();
                return View("Home", m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult GetUnassignedFTF(int page, int pageSize)
        {
            return Json(_ftfStatsService.GetUnassignedFTF(page, pageSize));
        }

        public ActionResult SetFTFCompleted(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return Content(_ftfStatsService.SetFTFCompleted(id) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }

        public ActionResult ClearFTFAssignment(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return Content(_ftfStatsService.ClearFTFAssignment(id) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }
        public ActionResult ClearSTFAssignment(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return Content(_ftfStatsService.ClearSTFAssignment(id) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }
        public ActionResult ClearTTFAssignment(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return Content(_ftfStatsService.ClearTTFAssignment(id) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }

        public ActionResult SetFTFAssignment(long geocacheId, long logId)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return Content(_ftfStatsService.SetFTFAssignment(geocacheId, logId) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }
        public ActionResult SetSTFAssignment(long geocacheId, long logId)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return Content(_ftfStatsService.SetSTFAssignment(geocacheId, logId) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }
        public ActionResult SetTTFAssignment(long geocacheId, long logId)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return Content(_ftfStatsService.SetTTFAssignment(geocacheId, logId) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }

        public ActionResult ResetFTFCounter(long id)
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                _ftfStatsService.ResetFTFCounter(id);
                return Content("OK");
            }
            else
            {
                return null;
            }
        }

        public ActionResult AddAllGeocachesToQueue()
        {
            if (Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                var list = _ftfStatsService.GetUnassignedFTF(1, 1000); //max 1000 to add to queue
                if (list.Geocaches.Count>0)
                {
                    _taskSchedulerService.AddScheduledWaypoint((from a in list.Geocaches select a.Code).ToList(), false);
                }
                return Content("OK");
            }
            else
            {
                return null;
            }
        }

    }
}
using Globalcaching.Services;
using Orchard;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class GeocacheDistanceController : Controller
    {
        private readonly IGeocacheDistanceService _geocacheDistanceService;
        public IOrchardServices Services { get; set; }

        public GeocacheDistanceController(IGeocacheDistanceService geocacheDistanceService,
            IOrchardServices services)
        {
            Services = services;
            _geocacheDistanceService = geocacheDistanceService;
        }

        [Themed]
        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.DistanceAdmin))
            {
                var m = _geocacheDistanceService.GetUnassignedDistance(1, 50);
                return View("Home", m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult SetDistanceChecked(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.DistanceAdmin))
            {
                return Content(_geocacheDistanceService.SetDistanceChecked(id) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }

        public ActionResult SetDistanceHandledBy(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.DistanceAdmin))
            {
                return Content(_geocacheDistanceService.SetDistanceHandledBy(id) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetUnassignedDistance(int page, int pageSize)
        {
            if (Services.Authorizer.Authorize(Permissions.DistanceAdmin))
            {
                return Json(_geocacheDistanceService.GetUnassignedDistance(page, pageSize));
            }
            else
            {
                return null;
            }
        }

        public ActionResult SetDistance(int id, string distance)
        {
            if (string.IsNullOrEmpty(distance))
            {
                return ClearDistance(id);
            }
            else
            {
                if (Services.Authorizer.Authorize(Permissions.DistanceAdmin))
                {
                    double d = Core.Helper.ConvertToDouble(distance);
                    return Content(_geocacheDistanceService.SetDistance(id, d) ? "OK" : "NOK");
                }
                else
                {
                    return null;
                }
            }
        }

        public ActionResult ClearDistance(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.DistanceAdmin))
            {
                return Content(_geocacheDistanceService.SetDistance(id, null) ? "OK" : "NOK");
            }
            else
            {
                return null;
            }
        }

    }
}
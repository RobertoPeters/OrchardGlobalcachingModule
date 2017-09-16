using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class GeocacheBatchLogController: Controller
    {
        private readonly IGeocacheBatchLogService _geocacheBatchLogService;

        public GeocacheBatchLogController(IGeocacheBatchLogService geocacheBatchLogService)
        {
            _geocacheBatchLogService = geocacheBatchLogService;
        }

        [HttpPost]
        public ActionResult GetGeocachesByOwner(string owner)
        {
            return Json(_geocacheBatchLogService.GetGeocachesByOwner(owner));
        }

        [HttpPost]
        public ActionResult GetGeocachesByName(string name)
        {
            return Json(_geocacheBatchLogService.GetGeocachesByName(name));
        }

        [HttpPost]
        public ActionResult GetGeocachesByCode(string code)
        {
            return Json(_geocacheBatchLogService.GetGeocachesByCode(code));
        }

    }
}
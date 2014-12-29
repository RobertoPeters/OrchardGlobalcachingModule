using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class CoordCheckerController: Controller
    {
        private ICoordCheckerService _coordCheckerService;

        public CoordCheckerController(ICoordCheckerService coordCheckerService)
        {
            _coordCheckerService = coordCheckerService;
        }

        [HttpPost]
        public ActionResult CheckCoord(string wp, string coord)
        {
            string remarks;
            bool ok = _coordCheckerService.CheckCoord(wp, coord, out remarks);
            return Json(new {OK= ok, Remarks= remarks});
        }
    }
}
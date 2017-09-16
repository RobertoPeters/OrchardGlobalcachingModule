using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class CoordCheckerController: Controller
    {
        private ICoordCheckerService _coordCheckerService;

        public CoordCheckerController(ICoordCheckerService coordCheckerService)
        {
            _coordCheckerService = coordCheckerService;
        }

        //[Themed]
        public ActionResult RedirectFromOldSite()
        {
            return Redirect(Request.Url.ToString().ToLower().Replace("caches/coordcheck.aspx", "CoordinatenChecker"));
        }

        [HttpPost]
        public ActionResult CheckCoord(string wp, string coord)
        {
            string remarks;
            bool ok = _coordCheckerService.CheckCoord(wp, coord, out remarks);
            return Json(new {OK= ok, Remarks= remarks});
        }

        [HttpPost]
        public ActionResult GetCode(string code)
        {
            return Json(_coordCheckerService.CoordCheckerMaintModel(code, 1, 20));
        }

        [HttpPost]
        public ActionResult DeleteCode(string code)
        {
            return Json(_coordCheckerService.DeleteCode(code, 1, 20));
        }

        [HttpPost]
        public ActionResult CreateCode(string code, string coord, string radius, string notiwrong, string noticorrect)
        {
            LatLon ll = LatLon.FromString(coord);
            if (ll != null && !string.IsNullOrEmpty(code) && code.Trim().Length>0)
            {
                GCEuCoordCheckCode m = new GCEuCoordCheckCode();
                m.Code = code.Trim();
                m.Lat = ll.lat;
                m.Lon = ll.lon;
                m.NotifyOnFailure = bool.Parse(notiwrong);
                m.NotifyOnSuccess = bool.Parse(noticorrect);
                m.Radius = int.Parse(radius);
                var result = _coordCheckerService.CreateCode(m, 1, 20);
                if (result != null)
                {
                    return Json(result);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdateCode(string code, string coord, string radius, string notiwrong, string noticorrect)
        {
            LatLon ll = LatLon.FromString(coord);
            if (ll != null && !string.IsNullOrEmpty(code) && code.Trim().Length > 0)
            {
                GCEuCoordCheckCode m = new GCEuCoordCheckCode();
                m.Code = code.Trim();
                m.Lat = ll.lat;
                m.Lon = ll.lon;
                m.NotifyOnFailure = bool.Parse(notiwrong);
                m.NotifyOnSuccess = bool.Parse(noticorrect);
                m.Radius = int.Parse(radius);
                return Json(_coordCheckerService.UpdateCode(m, 1, 20));
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetAttempts(string code, int page, int pageSize)
        {
            return Json(_coordCheckerService.CoordCheckerAttempts(code, page, pageSize));
        }

    }
}
using Globalcaching.Services;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class GCComUserSearchController : Controller
    {
        private readonly IGCComSearchUserService _gcComSearchUserService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GCComUserSearchController(IGCComSearchUserService gcComSearchUserService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcComSearchUserService = gcComSearchUserService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        [HttpPost]
        public ActionResult Index()
        {
            int page = 1;
            int mode = 0;
            int pageSize = 100;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "25", out pageSize);
            int.TryParse(Request["mode"] ?? "0", out mode);
            return Json(_gcComSearchUserService.GetGeocachingComUsers(mode, page, pageSize, Request["id"] as string));
        }

        [Themed]
        public ActionResult ShowGCComUserInfo(long id)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings!=null && settings.YafUserID>1)
            {
                return View("Home", _gcComSearchUserService.GetGeocacherInfo(id));
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

    }
}
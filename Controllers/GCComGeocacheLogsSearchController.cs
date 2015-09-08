using Globalcaching.Services;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class GCComGeocacheLogsSearchController : Controller
    {
        private readonly IGCComSearchGeocacheLogsService _gcComSearchLogsService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GCComGeocacheLogsSearchController(IGCComSearchGeocacheLogsService gcComSearchLogsService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcComSearchLogsService = gcComSearchLogsService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        [HttpPost]
        public ActionResult Index()
        {
            int page = 1;
            int pageSize = 50;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "50", out pageSize);
            return Json(_gcComSearchLogsService.GetGeocachingComLogsOfUser(page, pageSize, Request["id"] as string));
        }

        [Themed]
        public ActionResult FTFLogs(long id)
        {
            int page = 1;
            int pageSize = 25;
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return View("FTFLogs", _gcComSearchLogsService.GetFTFLogsOfUser(page, pageSize, id));
            }
            else
            {
                return View("DisplayTemplates/Parts.ForDonatorsOnly");
            }
        }

        [HttpPost]
        public ActionResult FTFLogsOfUser()
        {
            int page = 1;
            int pageSize = 25;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "25", out pageSize);
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return Json(_gcComSearchLogsService.GetFTFLogsOfUser(page, pageSize, long.Parse(Request["id"])));
            }
            else
            {
                return null;
            }
        }

        [Themed]
        public ActionResult STFLogs(long id)
        {
            int page = 1;
            int pageSize = 25;
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return View("STFLogs", _gcComSearchLogsService.GetSTFLogsOfUser(page, pageSize, id));
            }
            else
            {
                return View("DisplayTemplates/Parts.ForDonatorsOnly");
            }
        }

        [HttpPost]
        public ActionResult STFLogsOfUser()
        {
            int page = 1;
            int pageSize = 25;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "25", out pageSize);
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return Json(_gcComSearchLogsService.GetSTFLogsOfUser(page, pageSize, long.Parse(Request["id"])));
            }
            else
            {
                return null;
            }
        }

        [Themed]
        public ActionResult TTFLogs(long id)
        {
            int page = 1;
            int pageSize = 25;
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return View("TTFLogs", _gcComSearchLogsService.GetTTFLogsOfUser(page, pageSize, id));
            }
            else
            {
                return View("DisplayTemplates/Parts.ForDonatorsOnly");
            }
        }

        [HttpPost]
        public ActionResult TTFLogsOfUser()
        {
            int page = 1;
            int pageSize = 25;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "25", out pageSize);
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return Json(_gcComSearchLogsService.GetTTFLogsOfUser(page, pageSize, long.Parse(Request["id"])));
            }
            else
            {
                return null;
            }
        }
    }
}
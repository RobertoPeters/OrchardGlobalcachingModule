using Globalcaching.Services;
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

        public GCComGeocacheLogsSearchController(IGCComSearchGeocacheLogsService gcComSearchLogsService)
        {
            _gcComSearchLogsService = gcComSearchLogsService;
        }

        [HttpPost]
        public ActionResult Index()
        {
            int page = 1;
            int pageSize = 50;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "100", out pageSize);
            return Json(_gcComSearchLogsService.GetGeocachingComLogsOfUser(page, pageSize, Request["id"] as string));
        }
    }
}
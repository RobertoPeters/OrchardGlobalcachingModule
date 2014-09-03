using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class GCComUserSearchController : Controller
    {
        private readonly IGCComSearchUserService _gcComSearchUserService;

        public GCComUserSearchController(IGCComSearchUserService gcComSearchUserService)
        {
            _gcComSearchUserService = gcComSearchUserService;
        }

        [HttpPost]
        public ActionResult Index()
        {
            int page = 1;
            int pageSize = 100;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "100", out pageSize);
            return Json(_gcComSearchUserService.GetGeocachingComUsers(page, pageSize, Request["id"] as string));
        }
    }
}
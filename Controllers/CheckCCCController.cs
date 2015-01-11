using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class CheckCCCController: Controller
    {
        private readonly IGCEuCCCSettingsService _gcEuCCCSettingsService;

        public CheckCCCController(IGCEuCCCSettingsService gcEuCCCSettingsService)
        {
            _gcEuCCCSettingsService = gcEuCCCSettingsService;
        }

        [HttpPost]
        public ActionResult Index()
        {
            int page = 1;
            int pageSize = 50;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "50", out pageSize);
            return Json(_gcEuCCCSettingsService.GetCCCUsersForGeocache(page, pageSize, Request["id"] as string, true));
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult CCCCheck()
        {
            string gccode = Request.QueryString["wp"];
            string username = Request.QueryString["usr"];
            string password = Request.QueryString["pwd"];
            return Content(_gcEuCCCSettingsService.GetCCCServiceResult(gccode, username, password));
        }
    }
}
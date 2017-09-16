using Globalcaching.Services;
using Orchard;
using Orchard.Security;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class CheckCCCController: Controller
    {
        public IOrchardServices Services { get; set; }
        private readonly IGCEuCCCSettingsService _gcEuCCCSettingsService;

        public CheckCCCController(IGCEuCCCSettingsService gcEuCCCSettingsService,
            IOrchardServices services)
        {
            _gcEuCCCSettingsService = gcEuCCCSettingsService;
            Services = services;
        }

        public ActionResult RedirectFromOldSite()
        {
            return Redirect(Request.Url.ToString().ToLower().Replace("cachers/ccc.aspx", "CCCHulp"));
        }

        [HttpPost]
        public ActionResult Index()
        {
            var settings = _gcEuCCCSettingsService.GetSettings();
            if (settings != null && settings.Active)
            {
                int page = 1;
                int pageSize = 50;
                int.TryParse(Request["page"] ?? "1", out page);
                int.TryParse(Request["pageSize"] ?? "50", out pageSize);
                return Json(_gcEuCCCSettingsService.GetCCCUsersForGeocache(page, pageSize, Request["id"] as string, true));
            }
            else
            {
                return null;
            }
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult CCCCheck()
        {
            string gccode = Request.QueryString["wp"];
            string username = Request.QueryString["usr"];
            string password = Request.QueryString["pwd"];
            return Content(_gcEuCCCSettingsService.GetCCCServiceResult(gccode, username, password));
        }

        [Themed]
        public ActionResult ListCCCMembers()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return View("Home", _gcEuCCCSettingsService.GetAllCCCUsers());
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [Themed]
        [HttpPost]
        public ActionResult DeactivateCCCMember(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                _gcEuCCCSettingsService.DeactivateCCCMember(id);
                return RedirectToAction("ListCCCMembers");
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [Themed]
        public ActionResult GetRequests()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return View("Requests", _gcEuCCCSettingsService.GetRequestCalls(1, 500));
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpPost]
        public ActionResult GetRequestsPage(int page, int pageSize)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return Json(_gcEuCCCSettingsService.GetRequestCalls(page, pageSize));
            }
            else
            {
                return null;
            }
        }

    }
}
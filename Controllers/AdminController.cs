using Orchard;
using Orchard.Environment;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class AdminController: Controller
    {
        public IOrchardServices Services { get; set; }
        private IHostEnvironment _hostEnvironment;

        public AdminController(IOrchardServices services,
            IHostEnvironment hostEnvironment)
        {
            Services = services;
            _hostEnvironment = hostEnvironment;
        }

        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(StandardPermissions.AccessAdminPanel))
            {
                return View("Home");
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult Restart()
        {
            if (Services.Authorizer.Authorize(StandardPermissions.AccessAdminPanel))
            {
                _hostEnvironment.RestartAppDomain();
            }
            return null;
        }
    }
}
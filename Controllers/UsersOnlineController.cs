using Globalcaching.Services;
using Orchard;
using Orchard.Security;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class UsersOnlineController: Controller
    {
        private readonly IUsersOnlineService _usersOnlineService;
        public IOrchardServices Services { get; set; }

        public UsersOnlineController(IOrchardServices services, 
            IUsersOnlineService usersOnlineService)
        {
            Services = services;
            _usersOnlineService = usersOnlineService;
        }

        [Themed]
        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin) || Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                return View("Home", _usersOnlineService.GetUserActivity());
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }
    }
}
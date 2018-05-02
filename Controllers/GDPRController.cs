using Globalcaching.Core;
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
    [ValidateInput(false)]
    public class GDPRController: Controller
    {
        public IGDPRService _gdprService { get; set; }

        public GDPRController(IGDPRService gdprService)
        {
            _gdprService = gdprService;
        }


        public ActionResult DownloadData()
        {
            _gdprService.DownloadData(Response);
            return null;
        }

    }
}
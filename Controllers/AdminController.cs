using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Environment;
using Orchard.Localization;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class AdminController: Controller
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public IOrchardServices Services { get; set; }
        private IHostEnvironment _hostEnvironment;
        public Localizer T { get; set; }
        private ITaskSchedulerService _taskSchedulerService;

        public AdminController(IOrchardServices services,
            ITaskSchedulerService taskSchedulerService,
            IHostEnvironment hostEnvironment)
        {
            Services = services;
            _hostEnvironment = hostEnvironment;
            T = NullLocalizer.Instance;
            _taskSchedulerService = taskSchedulerService;
        }

        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return View("Home", _taskSchedulerService.GetSchedulerInfoModel());
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult Restart()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                _hostEnvironment.RestartAppDomain();
            }
            return null;
        }

        public ActionResult RefreshForParels()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("De geocaches worden bijgewerkt"));
                _taskSchedulerService.AddScheduledWaypointForParels();
                return View("Home", _taskSchedulerService.GetSchedulerInfoModel());
            }
            return null;
        }

        public ActionResult AddParel(string gccode)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                long id = 0;
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                {
                    id = db.Fetch<long>("select ID from GCComGeocache where Code=@0", gccode).FirstOrDefault();
                }
                if (id > 0)
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                    {
                        GCEuParel p = new GCEuParel();
                        p.GeocacheID = id;
                        db.Insert(p);
                    }
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Geocache is toegevoegs aan de parels"));
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Kan geocache code niet vinden"));
                }
            }
            return View("Home", _taskSchedulerService.GetSchedulerInfoModel());
        }

    }
}
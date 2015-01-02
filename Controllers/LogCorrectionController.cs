using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class LogCorrectionController : Controller
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly ITaskSchedulerService _taskSchedulerService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LogCorrectionController(IGCEuUserSettingsService gcEuUserSettingsService,
            ITaskSchedulerService taskSchedulerService,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _taskSchedulerService = taskSchedulerService;
            Services = services;
            T = NullLocalizer.Instance;
        }


        [Themed]
        public ActionResult SubmitGeocache(string gccode, string ReturnUrl)
        {
            var usrSettings = _gcEuUserSettingsService.GetSettings();
            if (usrSettings != null && usrSettings.YafUserID > 1)
            {
                if (!string.IsNullOrEmpty(gccode))
                {
                    if (gccode.ToUpper().StartsWith("GC"))
                    {
                        if (_taskSchedulerService.GetScheduledGeocaches().Count < 500)
                        {
                            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                            {
                                if (db.FirstOrDefault<GCComGeocache>("where Code=@0", gccode.ToUpper()) != null)
                                {
                                    List<string> l = new List<string>();
                                    l.Add(gccode.ToUpper());
                                    _taskSchedulerService.AddScheduledWaypoint(l, true);
                                }
                                else
                                {
                                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("De geocache is (nog) niet bekend in ons systeem."));
                                }
                            }
                        }
                        else
                        {
                            Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Er staan meer dan 500 geocaches in de wachtrij. Wacht een tijdje en probeer het opnieuw."));
                        }
                    }
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Geen geocache code opgegeven."));
                }
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Je bent niet ingelogd op globalcaching.eu."));
            }
            return Redirect(ReturnUrl);
        }


    }
}
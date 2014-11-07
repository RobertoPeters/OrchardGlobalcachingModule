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
    public class LogGCComController : Controller
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LogGCComController(IGCEuUserSettingsService gcEuUserSettingsService,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            Services = services;
            T = NullLocalizer.Instance;
        }


        [Themed]
        public ActionResult LogGeocache(string id)
        {
            var usrSettings = _gcEuUserSettingsService.GetSettings();
            if (usrSettings != null && usrSettings.YafUserID>1)
            {
                if (!string.IsNullOrEmpty(usrSettings.LiveAPIToken))
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        GCComGeocache gc = null; 
                        using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                        {
                            long l;
                            Guid g;
                            if (id.ToUpper().StartsWith("GC"))
                            {
                                gc = db.FirstOrDefault<GCComGeocache>("where Code=@0", id.ToUpper());
                            }
                            else if (long.TryParse(id,out l))
                            {
                                gc = db.FirstOrDefault<GCComGeocache>("where ID=@0", id);
                            }
                            else if (Guid.TryParse(id, out g))
                            {
                                gc = db.FirstOrDefault<GCComGeocache>("where GUID=@0", id);
                            }
                        }
                        if (gc != null)
                        {
                            return View("LogGeocache", gc);
                        }
                        else
                        {
                            Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("De opgegeven geocache staat (nog) niet in onze database."));
                            return View("NotAuthorized");
                        }
                    }
                    else
                    {
                        Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Geen geocache code opgegeven."));
                        return View("NotAuthorized");
                    }
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Live API niet geautoriseerd."));
                    return View("NotAuthorized");
                }
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Je bent niet ingelogd op globalcaching.eu."));
                return View("NotAuthorized");
            }
        }

        [HttpPost]
        public ActionResult LogGC(string gcid, DateTime visitDate, string logText, bool favorite)
        {
            string[] result = new string[3];
            result[0] = "ERROR";
            var usrSettings = _gcEuUserSettingsService.GetSettings();
            if (usrSettings != null && usrSettings.YafUserID > 1 && !string.IsNullOrEmpty(usrSettings.LiveAPIToken))
            {
                var log = LiveAPIClient.LogGeocache(usrSettings.LiveAPIToken, gcid, logText, visitDate.Date, favorite);
                if (log != null)
                {
                    result[0] = "OK";
                    result[1] = Url.Action("Index", "DisplayGeocache", new { id = gcid });
                    result[2] = log.Url;
                }
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult LogTB(string tbsToLog, string tbsLogged, DateTime visitDate, string logText)
        {
            string[] result = new string[3];
            result[0] = "ERROR";
            var usrSettings = _gcEuUserSettingsService.GetSettings();
            if (usrSettings != null && usrSettings.YafUserID > 1 && !string.IsNullOrEmpty(usrSettings.LiveAPIToken))
            {
                string sbToLog = tbsToLog;
                string sbLogged = tbsLogged;

                string[] tbl = tbsToLog.Split(new char[] { ' ', ',', '\t', '\r', '\n', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (tbl.Length > 0)
                {
                    string tb = tbl[0].ToUpper();
                    if (!tb.StartsWith("TB"))
                    {
                        if (LiveAPIClient.LogTrackable(usrSettings.LiveAPIToken, tb.ToUpper(), logText, visitDate.Date))
                        {
                            //if succeeded
                            if (tbl.Length > 0)
                            {
                                sbToLog = string.Join(", ", tbl.Skip(1).ToArray());
                            }
                            else
                            {
                                sbToLog = "";
                            }
                            sbLogged = string.Concat(sbLogged, tb, "\r\n");
                            result[0] = "OK";
                            result[1] = sbToLog.ToString();
                            result[2] = sbLogged.ToString();
                        }
                    }
                }
            }
            return Json(result);
        }

    }
}
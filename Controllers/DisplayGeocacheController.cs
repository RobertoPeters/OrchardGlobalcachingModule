using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class DisplayGeocacheController : Controller
    {
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public DisplayGeocacheController(IGCEuUserSettingsService gcEuUserSettingsService,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index(string id)
        {
            var usrSettings = _gcEuUserSettingsService.GetSettings();
            if (usrSettings!=null && usrSettings.ShowGeocachesOnGlobal == true)
            {
                if (!string.IsNullOrEmpty(usrSettings.LiveAPIToken))
                {
                    GeocacheDataModel data = GetGeocacheData(id, 5);
                    if (data != null)
                    {
                        return View("Home", data);
                    }
                    else
                    {
                        //unknown geocache
                        return new RedirectResult(string.Format("http://coord.info/{0}", id));
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
                //redirect
                return new RedirectResult(string.Format("http://coord.info/{0}", id));
            }
        }

        public GeocacheDataModel GetGeocacheData(string geocacheCode, int maxLogs)
        {
            GeocacheDataModel result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                GCComGeocache comGcData = db.FirstOrDefault<GCComGeocache>("where Code=@0", geocacheCode);
                if (comGcData != null)
                {
                    string gcEuDatabase = Core.Helper.GetTableNameFromConnectionString(dbGcEuDataConnString);

                    result = new GeocacheDataModel();
                    result.GCComGeocacheData = comGcData;
                    result.GCEuGeocacheData = db.FirstOrDefault<GCEuGeocache>(string.Format("select * from [{0}].[dbo].[GCEuGeocache] where ID=@0", gcEuDatabase), comGcData.ID);
                    result.GCComGeocacheLogs = db.Fetch<GCComGeocacheLog, GCComUser, GeocacheLogInfo>(
        (l, u) => { l.FinderId = u.ID; return new GeocacheLogInfo(l, u); },
        string.Format(@"SELECT TOP {0} * FROM GCComGeocacheLog with (nolock) LEFT JOIN GCComUser with (nolock) ON GCComGeocacheLog.FinderId = GCComUser.ID WHERE GCComGeocacheLog.GeocacheID=@0 ORDER BY GCComGeocacheLog.VisitDate desc, GCComGeocacheLog.ID desc", maxLogs), comGcData.ID);
                }
            }
            return result;
        }

    }
}
﻿using Globalcaching.Models;
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
        private IWorkContextAccessor _workContextAccessor;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public DisplayGeocacheController(IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            Services = services;
            _workContextAccessor = workContextAccessor;
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
                    GeocacheDataModel data = GetGeocacheData(id, _workContextAccessor.GetContext().HttpContext.Request.QueryString["al"]==null? 5: 30000);
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

        public ActionResult GetAllLogs(int id)
        {
            List<GeocacheLogInfo> result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = GetLogsOfGeocache(db, id, 30000);
            }
            return Json(result);
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
                    result.Owner = db.FirstOrDefault<GCComUser>("where ID=@0", result.GCComGeocacheData.OwnerId);
                    if (result.GCEuGeocacheData != null)
                    {
                        if (result.GCEuGeocacheData.FTFUserID != null)
                        {
                            result.FTF = db.FirstOrDefault<GCComUser>("where ID=@0", result.GCEuGeocacheData.FTFUserID);
                        }
                        if (result.GCEuGeocacheData.STFUserID != null)
                        {
                            result.STF = db.FirstOrDefault<GCComUser>("where ID=@0", result.GCEuGeocacheData.STFUserID);
                        }
                        if (result.GCEuGeocacheData.TTFUserID != null)
                        {
                            result.TTF = db.FirstOrDefault<GCComUser>("where ID=@0", result.GCEuGeocacheData.TTFUserID);
                        }
                    }
                    result.LogTypes = db.Fetch<GCComLogType>("");
                    result.GCComGeocacheLogs = GetLogsOfGeocache(db, comGcData.ID, maxLogs);
                }
            }
            return result;
        }

        public List<GeocacheLogInfo> GetLogsOfGeocache(PetaPoco.Database db, long geocacheID, int maxLogs)
        {
            return db.Fetch<GCComGeocacheLog, GCComUser, GeocacheLogInfo>(
                (l, u) => { l.FinderId = u.ID; return new GeocacheLogInfo(l, u); },
                string.Format(@"SELECT TOP {0} * FROM GCComGeocacheLog with (nolock) LEFT JOIN GCComUser with (nolock) ON GCComGeocacheLog.FinderId = GCComUser.ID WHERE GCComGeocacheLog.GeocacheID=@0 ORDER BY GCComGeocacheLog.VisitDate desc, GCComGeocacheLog.ID desc", maxLogs), geocacheID);
        }
    }
}
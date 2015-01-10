using Globalcaching.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class LiveAPIDownloadController: Controller
    {
        private readonly ILiveAPIDownloadService _liveAPIDownloadService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LiveAPIDownloadController(ILiveAPIDownloadService liveAPIDownloadService,
            IOrchardServices services)
        {
            _liveAPIDownloadService = liveAPIDownloadService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [HttpPost]
        public ActionResult GetLiveAPIDownloadStatus()
        {
            var m = _liveAPIDownloadService.DownloadStatus;
            if (m == null)
            {
                return null;
            }
            else
            {
                return Json(m);
            }
        }

        [HttpPost]
        public ActionResult UpdateLiveAPILimits()
        {
            var m = _liveAPIDownloadService.UpdateLiveAPILimits();
            if (m == null)
            {
                return null;
            }
            else
            {
                return Json(m);
            }
        }

        [HttpPost]
        public ActionResult StartDownload(bool isLite, string fileFormat)
        {
            var m = _liveAPIDownloadService.StartDownload(isLite, fileFormat);
            if (m == null)
            {
                return null;
            }
            else
            {
                return Json(m);
            }
        }

        [HttpPost]
        public ActionResult StopDownload()
        {
            var m = _liveAPIDownloadService.StopDownload();
            if (m == null)
            {
                return null;
            }
            else
            {
                return Json(m);
            }
        }

        [Themed]
        public ActionResult CopyMacroResultToDownload()
        {
            if (_liveAPIDownloadService.SetMacroResultForDownload())
            {
                var m = _liveAPIDownloadService.DownloadStatus;
                if (m == null)
                {
                    return HttpNotFound("Fout bij het ophalen van de download status");
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("De geocaches kunnen worden gedownload."));
                    return View("../DisplayTemplates/Parts.LiveAPIDownload", m);
                }
            }
            else
            {
                var m = _liveAPIDownloadService.DownloadStatus;
                if (m == null)
                {
                    return HttpNotFound("Fout bij het ophalen van de download status");
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Er is een fout opgetreden. Controlleer of er al geocaches gedownload worden."));
                    //return View("~/Views/DisplayTemplates/Parts.LiveAPIDownload", m);
                    return View("../DisplayTemplates/Parts.LiveAPIDownload", m);
                }
            }
        }

        public FileResult DownloadFile()
        {
            string filename = _liveAPIDownloadService.GetDownloadFilePath();
            string contentType = "application/gpx+xml";
            return File(filename, contentType, string.Format("{0}.gpx", DateTime.Now.ToString("yyyMMddhhmmss")));
        }

        public FileResult DownloadGeocache(string id)
        {
            string filename = _liveAPIDownloadService.DownloadGPX(id);
            string contentType = "application/gpx+xml";
            return File(filename, contentType, string.Format("{0}.gpx", id));
        }
    }
}
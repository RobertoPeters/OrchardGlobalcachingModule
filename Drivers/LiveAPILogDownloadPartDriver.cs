using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class LiveAPILogDownloadDriver : ContentPartDriver<LiveAPILogDownloadPart>
    {
        private readonly ILiveAPIDownloadService _liveAPIDownloadService;
        private readonly IWorkContextAccessor _workContextAccessor;

        protected override string Prefix { get { return ""; } }
        private readonly IGCEuUserSettingsService _userSettingsService;

        public LiveAPILogDownloadDriver(ILiveAPIDownloadService liveAPIDownloadService,
            IWorkContextAccessor workContextAccessor,
            IGCEuUserSettingsService userSettingsService)
        {
            _liveAPIDownloadService = liveAPIDownloadService;
            _workContextAccessor = workContextAccessor;
            _userSettingsService = userSettingsService;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        protected override DriverResult Display(LiveAPILogDownloadPart part, string displayType, dynamic shapeHelper)
        {
            var m = _userSettingsService.GetSettings();
            if (m != null && m.IsDonator)
            {
                var m2 = _liveAPIDownloadService.DownloadLogStatus;
                if (m2 == null)
                {
                    m2 = new GCEuDownloadLogsStatus();
                    m2.Busy = false;
                    m2.IncludeYourArchived = false;
                    m2.LastUpdateAt = DateTime.Now;
                    m2.LogTableName = "";
                    m2.RequestedAt = DateTime.Now;
                    m2.Status = "Nog geen log download verzoek gedaan.";
                    m2.TotalFindCount = null;
                    m2.TotalLogsImported = 0;
                    m2.UserID = m.YafUserID;
                    m2.UserNameBusy = "";
                    m2.UserNames = "";
                    m2.UserNamesCompleted = "";
                }
                return ContentShape("Parts_LiveAPILogDownload",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.LiveAPILogDownload",
                                Model: m2,
                                Prefix: Prefix));
            }
            else
            {
                return ContentShape("Parts_ForDonatorsOnly",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.ForDonatorsOnly",
                                Model: null,
                                Prefix: Prefix));
            }
        }
    }
}
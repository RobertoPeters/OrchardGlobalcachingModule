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
    public class LiveAPIDownloadDriver : ContentPartDriver<LiveAPIDownloadPart>
    {
        private readonly ILiveAPIDownloadService _liveAPIDownloadService;
        private readonly IWorkContextAccessor _workContextAccessor;

        protected override string Prefix { get { return ""; } }

        public LiveAPIDownloadDriver(ILiveAPIDownloadService liveAPIDownloadService,
            IWorkContextAccessor workContextAccessor)
        {
            _liveAPIDownloadService = liveAPIDownloadService;
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        protected override DriverResult Display(LiveAPIDownloadPart part, string displayType, dynamic shapeHelper)
        {
            var m = _liveAPIDownloadService.DownloadStatus;
            return ContentShape("Parts_LiveAPIDownload",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.LiveAPIDownload",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
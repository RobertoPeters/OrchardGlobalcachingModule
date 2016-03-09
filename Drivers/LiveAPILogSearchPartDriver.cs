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
    public class LiveAPILogSearchDriver : ContentPartDriver<LiveAPILogSearchPart>
    {
        private readonly ILiveAPIDownloadService _liveAPIDownloadService;
        protected override string Prefix { get { return ""; } }

        public LiveAPILogSearchDriver(ILiveAPIDownloadService liveAPIDownloadService)
        {
            _liveAPIDownloadService = liveAPIDownloadService;
        }

        protected override DriverResult Display(LiveAPILogSearchPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_LiveAPILogSearch",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.LiveAPILogSearch",
                            Model: _liveAPIDownloadService.GetLogs(1,50),
                            Prefix: Prefix));
        }
    }
}
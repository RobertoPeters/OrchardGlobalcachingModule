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
            /*
            CheckCCCResult m;
            string wp = HttpContext.Request.QueryString["wp"];
            if (!string.IsNullOrEmpty(wp))
            {
                m = _cccSettingsService.GetCCCUsersForGeocache(1, 50, wp, true);
            }
            else
            {
                m = new CheckCCCResult();
                m.GeocacheCode = "";
                m.TotalCount = 0;
                m.PageCount = 1;
                m.CurrentPage = 1;
            }
             * */
            return ContentShape("Parts_LiveAPIDownload",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.LiveAPIDownload",
                            Model: null,
                            Prefix: Prefix));
        }
    }
}
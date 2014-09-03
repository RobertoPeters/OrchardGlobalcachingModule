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
    public class LiveAPISettingsPartDriver : ContentPartDriver<LiveAPISettingsPart>
    {
        private readonly ILiveAPISettingsService _liveAPISettingsService;

        protected override string Prefix { get { return ""; } }

        public LiveAPISettingsPartDriver(ILiveAPISettingsService liveAPISettingsService)
        {
            _liveAPISettingsService = liveAPISettingsService;
        }

        protected override DriverResult Display(LiveAPISettingsPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _liveAPISettingsService.GetGeocachingComAccountInfo();
            if (settings == null)
            {
                settings = new GCComUser();
                settings.ID = -1;
                settings.UserName = "";
                settings.AvatarUrl = "";
            }

            return ContentShape("Parts_LiveAPISettings",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.LiveAPISettings",
                            Model: settings,
                            Prefix: Prefix));
        }
    }
}
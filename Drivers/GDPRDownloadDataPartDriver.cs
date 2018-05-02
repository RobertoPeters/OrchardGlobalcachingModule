using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GDPRDownloadDataPartDriver : ContentPartDriver<GDPRDownloadDataPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;

        protected override string Prefix { get { return ""; } }

        public GDPRDownloadDataPartDriver(IGCEuUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        protected override DriverResult Display(GDPRDownloadDataPart part, string displayType, dynamic shapeHelper)
        {
            var m = _userSettingsService.GetSettings();
            return ContentShape("Parts_GDPRDownloadData",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GDPRDownloadData",
                            Model: m,
                            Prefix: Prefix));
        }

    }
}
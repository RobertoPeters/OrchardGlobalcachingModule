using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class CoordCheckerMaintPartDriver : ContentPartDriver<CoordCheckerMaintPart>
    {
        private ICoordCheckerService _coordCheckerService;
        private IGCEuUserSettingsService _gcEuUserSettingsService;

        protected override string Prefix { get { return ""; } }

        public CoordCheckerMaintPartDriver(ICoordCheckerService coordCheckerService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _coordCheckerService = coordCheckerService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(CoordCheckerMaintPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                CoordCheckerMaintModel m = _coordCheckerService.CoordCheckerMaintModel(null, 1, 20);
                return ContentShape("Parts_CoordCheckerMaint",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.CoordCheckerMaint",
                                Model: m,
                                Prefix: Prefix));
            }
            else 
            {
                return null;
            }
        }

    }
}
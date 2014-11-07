using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GCEuGeocacheFilterMacroPartDriver : ContentPartDriver<GCEuGeocacheFilterMacroPart>
    {
        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        public readonly IMacroService _macroService;
        
        protected override string Prefix { get { return ""; } }

        public GCEuGeocacheFilterMacroPartDriver(IGCEuUserSettingsService gcEuUserSettingsService,
            IMacroService macroService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _macroService = macroService;
        }

        protected override DriverResult Display(GCEuGeocacheFilterMacroPart part, string displayType, dynamic shapeHelper)
        {
            var m = new GeocacheFilterMacroModel();
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings!=null && settings.YafUserID>1)
            {
                m.Macros = _macroService.GetMacrosOfUser(settings.YafUserID);
                m.Functions = _macroService.GetFunctions();
            }
            return ContentShape("Parts_GeocacheFilterMacro",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GeocacheFilterMacro",
                            Model: m,
                            Prefix: Prefix));
        }

    }
}
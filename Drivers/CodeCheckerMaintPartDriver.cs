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
    public class CodeCheckerMaintPartDriver : ContentPartDriver<CodeCheckerMaintPart>
    {
        private ICodeCheckerService _codeCheckerService;
        private IGCEuUserSettingsService _gcEuUserSettingsService;

        protected override string Prefix { get { return ""; } }

        public CodeCheckerMaintPartDriver(ICodeCheckerService codeCheckerService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _codeCheckerService = codeCheckerService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(CodeCheckerMaintPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                CodeCheckerMaintModel m = _codeCheckerService.CodeCheckerMaintModel(null, 1, 20);
                return ContentShape("Parts_CodeCheckerMaint",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.CodeCheckerMaint",
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
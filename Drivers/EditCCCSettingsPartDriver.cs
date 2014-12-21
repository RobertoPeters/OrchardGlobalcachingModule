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
    public class EditCCCSettingsPartDriver : ContentPartDriver<GCEuCCCSettingsPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;
        private readonly IGCEuCCCSettingsService _cccSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        protected override string Prefix { get { return ""; } }

        public EditCCCSettingsPartDriver(IGCEuUserSettingsService userSettingsService,
            IGCEuCCCSettingsService cccSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _userSettingsService = userSettingsService;
            _cccSettingsService = cccSettingsService;
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        protected override DriverResult Display(GCEuCCCSettingsPart part, string displayType, dynamic shapeHelper)
        {
            var settings = new EditCCCSettingsModel();
            settings.UserSettings = _userSettingsService.GetSettings();
            if (settings.UserSettings != null && settings.UserSettings.YafUserID > 1)
            {
                settings.CCCSettings = _cccSettingsService.GetSettings();
                if (settings.CCCSettings == null)
                {
                    settings.CCCSettings = new GCEuCCCUser();
                    settings.CCCSettings.Active = false;
                    settings.CCCSettings.Comment = "";
                    settings.CCCSettings.GCComUserID = settings.UserSettings.GCComUserID ?? 0;
                    settings.CCCSettings.HideEmailAddress = false;
                    settings.CCCSettings.ModifiedAt = DateTime.Now;
                    settings.CCCSettings.PreferSMS = false;
                    settings.CCCSettings.RegisteredAt = DateTime.Now;
                    settings.CCCSettings.SMS = true;
                    settings.CCCSettings.Telnr = "";
                    settings.CCCSettings.TwitterUsername = "";
                    settings.CCCSettings.UserID = settings.UserSettings.YafUserID;
                    settings.CCCSettings.UsersHelped = 0;
                }
            }
            settings.ReturnUrl = HttpContext.Request.Url.ToString();

            return ContentShape("Parts_EditCCCSettings",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.EditCCCSettings",
                            Model: settings,
                            Prefix: Prefix));
        }
    }
}
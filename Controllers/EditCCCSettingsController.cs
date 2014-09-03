using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class EditCCCSettingsController : Controller
    {
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IGCEuCCCSettingsService _gcEuCCCSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public EditCCCSettingsController(IGCEuUserSettingsService gcEuUserSettingsService,
            IGCEuCCCSettingsService gcEuCCCSettingsService,
            IWorkContextAccessor workContextAccessor,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _gcEuCCCSettingsService = gcEuCCCSettingsService;
            _workContextAccessor = workContextAccessor;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [HttpPost]
        public ActionResult Update(EditCCCSettingsModel settings)
        {
            bool success = false;
            if (ModelState.IsValid)
            {
                if (settings!=null && settings.CCCSettings != null)
                {
                    var usrSettings = _gcEuUserSettingsService.GetSettings();
                    var orgCCCSettings = _gcEuCCCSettingsService.GetSettings();
                    if (usrSettings != null && usrSettings.GCComUserID > 1 && usrSettings.YafUserID>1)
                    {
                        if (orgCCCSettings == null)
                        {
                            orgCCCSettings = new Models.GCEuCCCUser();
                            orgCCCSettings.UserID = usrSettings.YafUserID;
                            orgCCCSettings.UsersHelped = 0;
                            orgCCCSettings.RegisteredAt = DateTime.Now;
                        }
                        orgCCCSettings.Comment = settings.CCCSettings.Comment ?? "";
                        orgCCCSettings.Active = settings.CCCSettings.Active;
                        orgCCCSettings.ModifiedAt = DateTime.Now;
                        orgCCCSettings.GCComUserID = (long)usrSettings.GCComUserID;
                        orgCCCSettings.HideEmailAddress = settings.CCCSettings.HideEmailAddress;
                        orgCCCSettings.PreferSMS = settings.CCCSettings.PreferSMS;
                        orgCCCSettings.SMS = settings.CCCSettings.SMS;
                        orgCCCSettings.Telnr = settings.CCCSettings.Telnr ?? "";
                        orgCCCSettings.TwitterUsername = settings.CCCSettings.TwitterUsername ?? "";

                        _gcEuCCCSettingsService.UpdateSettings(orgCCCSettings);
                        success = true;
                    }
                }
            }
            if (success)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Instellingen zijn opgeslagen"));
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Er is een fout opgetreden. De instellingen zijn niet opgeslagen"));
            }
            return Redirect(settings.ReturnUrl);
        }

    }
}
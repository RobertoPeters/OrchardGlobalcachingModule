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
    public class ContactFormPartDriver : ContentPartDriver<ContactFormPart>
    {
        private readonly IGCEuUserSettingsService _userSettingsService;

        protected override string Prefix { get { return ""; } }

        public ContactFormPartDriver(IGCEuUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        protected override DriverResult Display(ContactFormPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _userSettingsService.GetSettings();
            var m = new GCEuContactForm();
            if (settings != null && settings.YafUserID>1)
            {
                m.EMail = _userSettingsService.GetEMail(settings.YafUserID);

            }
            return ContentShape("Parts_ContactForm",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.ContactForm",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
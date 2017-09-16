using Globalcaching.Models;
using Globalcaching.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class ContactFormController: Controller
    {
        public IContactFormService _contactFormServices { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ContactFormController(IContactFormService contactFormServices, IOrchardServices services)
        {
            _contactFormServices = contactFormServices;
            Services = services;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return View("Home", _contactFormServices.GetContactForms(1, 20));
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetContactForms(int page, int pageSize)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return Json(_contactFormServices.GetContactForms(page, pageSize));
            }
            else
            {
                return null;
            }
        }

        [Themed]
        [HttpPost]
        public ActionResult Update(GCEuContactForm m, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                _contactFormServices.SubmitContactForm(m);
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Het contactformulier is verstuurd en we zullen zo spoedig mogelijk antwoorden."));
                return Redirect("~/");
            }
            return View("../DisplayTemplates/Parts.ContactForm", m);
        }
    }
}
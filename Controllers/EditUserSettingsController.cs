using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement;
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
    public class EditUserSettingsController : Controller
    {
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public EditUserSettingsController(IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor,
            IOrchardServices services)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
            Services = services;
            T = NullLocalizer.Instance;
        }

        //private HttpContextBase HttpContext
        //{
        //    get { return _workContextAccessor.GetContext().HttpContext; }
        //}

        [HttpPost]
        public ActionResult Update(EditUserSettingsModel settings)
        {
            if (ModelState.IsValid)
            {
                var set = _gcEuUserSettingsService.GetSettings();
                if (settings!=null)
                {
                    set.DefaultCountryCode = settings.DefaultCountryCode;
                    set.ShowGeocachesOnGlobal = settings.ShowGeocachesOnGlobal;
                    set.MarkLogTextColor1 = settings.MarkLogTextColor1;
                    set.MarkLogTextColor2 = settings.MarkLogTextColor2;
                    set.MarkLogTextColor3 = settings.MarkLogTextColor3;
                    Core.LatLon ll = Core.Helper.GetLocation(settings.Homelocation);
                    if (ll == null)
                    {
                        set.HomelocationLat = null;
                        set.HomelocationLon = null;
                    }
                    else
                    {
                        set.HomelocationLat = ll.lat;
                        set.HomelocationLon = ll.lon;
                    }
                    _gcEuUserSettingsService.UpdateSettings(set);
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Instellingen zijn opgeslagen"));
                }
            }
            return Redirect(settings.ReturnUrl);
        }

        [Themed]
        public ActionResult ListMemberSettings()
        {
            if (Services.Authorizer.Authorize(StandardPermissions.AccessAdminPanel))
            {
                return View("Home", _gcEuUserSettingsService.GetAllMemberSettings());
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }
    }
}
using Globalcaching.Services;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class NewestCachesController: Controller
    {
        public readonly IGCEuUserSettingsService _gcEuUserSettingsService = null;

        public NewestCachesController(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult SetMode(int mode, string returnurl)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null)
            {
                settings.NewestCachesMode = mode;
                _gcEuUserSettingsService.UpdateSettings(settings);
            }
            return Redirect(returnurl);
        }

    }
}
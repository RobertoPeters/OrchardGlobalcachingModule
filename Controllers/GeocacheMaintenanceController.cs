using Globalcaching.Services;
using Globalcaching.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class GeocacheMaintenanceController: Controller
    {
        private readonly IGeocacheMaintenanceService _geocacheMaintenanceService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GeocacheMaintenanceController(IGeocacheMaintenanceService geocacheMaintenanceService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _geocacheMaintenanceService = geocacheMaintenanceService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public ActionResult GetGeocacheMaintenanceInfo(string id)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return Json(_geocacheMaintenanceService.GetGeocacheMaintenanceInfo(id));
            }
            else
            {
                return null;
            }
        }
    }
}
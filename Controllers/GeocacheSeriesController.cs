using Globalcaching.Services;
using Globalcaching.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class GeocacheSeriesController: Controller
    {
        private readonly IGeocacheSeriesService _geocacheSeriesService;

        public GeocacheSeriesController(IGeocacheSeriesService geocacheSeriesService)
        {
            _geocacheSeriesService = geocacheSeriesService;
        }

        public ActionResult GetGeocacheSeries(int page, int pageSize, int countryId, int startLength, int endLength)
        {
            var filter = new GeocacheSeriesFilter();
            filter.CountryID = countryId;
            filter.BeginLength = startLength;
            filter.EndLength = endLength;
            return Json(_geocacheSeriesService.GetGeocacheSeries(page, pageSize, filter));
        }
    }
}
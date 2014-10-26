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
    public class GeocacheSeriesPartDriver : ContentPartDriver<GeocacheSeriesPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IGeocacheSeriesService _geocacheSeriesService;

        public GeocacheSeriesPartDriver(IGeocacheSeriesService geocacheSeriesService)
        {
            _geocacheSeriesService = geocacheSeriesService;
        }

        protected override DriverResult Display(GeocacheSeriesPart part, string displayType, dynamic shapeHelper)
        {
            GeocacheSeriesFilter filter = new GeocacheSeriesFilter();
            filter.CountryID = 141;
            filter.BeginLength = 10;
            filter.EndLength = 0;
            var m = _geocacheSeriesService.GetGeocacheSeries(1, 50, filter);
            return ContentShape("Parts_GeocacheSeries",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GeocacheSeries",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
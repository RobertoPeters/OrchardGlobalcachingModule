using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GeocacheTypeStatsPartDriver : ContentPartDriver<GeocacheTypeStatsPart>
    {    
        private readonly IStatisticsServiceService _statisticsServiceService;

        protected override string Prefix { get { return ""; } }

        public GeocacheTypeStatsPartDriver(IStatisticsServiceService statisticsServiceService)
        {
            _statisticsServiceService = statisticsServiceService;
        }

        protected override DriverResult Display(GeocacheTypeStatsPart part, string displayType, dynamic shapeHelper)
        {
            var m = _statisticsServiceService.GetGeocacheTyeStats();
            return ContentShape("Parts_GeocacheTypeStats",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GeocacheTypeStats",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
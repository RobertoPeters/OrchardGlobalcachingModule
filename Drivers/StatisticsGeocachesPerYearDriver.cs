using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class StatisticsGeocachesPerYearDriver : ContentPartDriver<StatisticsGeocachesPerYearPart>
    {    
        public class ModelData
        {
            public List<StatisticsGeocachesPerYear> nlAll { get; set; }
            public List<StatisticsGeocachesPerYear> nlOnline { get; set; }
            public List<StatisticsGeocachesPerYear> beAll { get; set; }
            public List<StatisticsGeocachesPerYear> beOnline { get; set; }
            public List<StatisticsGeocachesPerYear> luAll { get; set; }
            public List<StatisticsGeocachesPerYear> luOnline { get; set; }
        }

        private readonly IStatisticsServiceService _statisticsServiceService;

        protected override string Prefix { get { return ""; } }

        public StatisticsGeocachesPerYearDriver(IStatisticsServiceService statisticsServiceService)
        {
            _statisticsServiceService = statisticsServiceService;
        }

        protected override DriverResult Display(StatisticsGeocachesPerYearPart part, string displayType, dynamic shapeHelper)
        {
            ModelData m = new ModelData();
            m.nlAll = _statisticsServiceService.GetGeocachesPerYear(true, 141);
            m.nlOnline = _statisticsServiceService.GetGeocachesPerYear(false, 141);
            m.beAll = _statisticsServiceService.GetGeocachesPerYear(true, 4);
            m.beOnline = _statisticsServiceService.GetGeocachesPerYear(false, 4);
            m.luAll = _statisticsServiceService.GetGeocachesPerYear(true, 8);
            m.luOnline = _statisticsServiceService.GetGeocachesPerYear(false, 8);
            return ContentShape("Parts_StatisticsGeocachesPerYear",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.StatisticsGeocachesPerYear",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
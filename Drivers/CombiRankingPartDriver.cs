using Globalcaching.Models;
using Globalcaching.Services;
using Orchard;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class CombiRankingPartDriver : ContentPartDriver<CombiRankingPart>
    {
        private readonly ICombiRankingService _combiRankingService;

        public CombiRankingPartDriver(ICombiRankingService combiRankingService)
        {
            _combiRankingService = combiRankingService;
        }

        protected override DriverResult Display(CombiRankingPart part, string displayType, dynamic shapeHelper)
        {
            var m = _combiRankingService.GetRanking(1, 50, 0, DateTime.Now.Year, null);
            return ContentShape("Parts_CombiRanking",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.CombiRanking",
                            Model: m,
                            Prefix: Prefix));
        }

    }
}
using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class FeatureTilesPartDriver : ContentPartDriver<FeatureTilesPart>
    {
        public FeatureTilesPartDriver()
        {
        }

        protected override DriverResult Display(FeatureTilesPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FeatureTiles",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.FeatureTiles",
                            Model: null,
                            Prefix: Prefix));
        }
    }

}
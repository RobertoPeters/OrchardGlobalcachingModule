using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class TrackableBatchLogDriver : ContentPartDriver<TrackableBatchLogPart>
    {
        public TrackableBatchLogDriver()
        {
        }

        protected override DriverResult Display(TrackableBatchLogPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_TrackableBatchLog",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.TrackableBatchLog",
                            Model: null,
                            Prefix: Prefix));
        }
    }
}
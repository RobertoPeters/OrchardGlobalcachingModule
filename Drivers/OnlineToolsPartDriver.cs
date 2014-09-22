using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class OnlineToolsPartDriver : ContentPartDriver<OnlineToolsPart>
    {
        public OnlineToolsPartDriver()
        {
        }

        protected override DriverResult Display(OnlineToolsPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_OnlineTools",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.OnlineTools",
                            Model: null,
                            Prefix: Prefix));
        }

    }
}
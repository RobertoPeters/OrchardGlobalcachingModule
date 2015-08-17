using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class ForDonatorsOnlyPartDriver : ContentPartDriver<ForDonatorsOnlyPart>
    {
        public ForDonatorsOnlyPartDriver()
        {
        }

        protected override DriverResult Display(ForDonatorsOnlyPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_ForDonatorsOnly",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.ForDonatorsOnly",
                            Model: null,
                            Prefix: Prefix));
        }
    }

}
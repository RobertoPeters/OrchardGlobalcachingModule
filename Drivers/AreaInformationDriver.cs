using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class AreaInformationDriver : ContentPartDriver<AreaInformationPart>
    {
        public AreaInformationDriver()
        {
            var i = Core.SHP.ShapeFilesManager.Instance;
        }

        protected override DriverResult Display(AreaInformationPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_AreaInformation",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.AreaInformation",
                            Model: null,
                            Prefix: Prefix));
        }
    }

}
using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class SelectionBuilderPartDriver : ContentPartDriver<SelectionBuilderPart>
    {
        public SelectionBuilderPartDriver()
        {
        }

        protected override DriverResult Display(SelectionBuilderPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_SelectionBuilder",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.SelectionBuilder",
                            Model: null,
                            Prefix: Prefix));
        }
    }

}
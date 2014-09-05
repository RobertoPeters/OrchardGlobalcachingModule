using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class SearchByAttributesPartDriver : ContentPartDriver<SearchByAttributesPart>
    {
        protected override string Prefix { get { return ""; } }

        public SearchByAttributesPartDriver()
        {
        }

        protected override DriverResult Display(SearchByAttributesPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_SearchByAttributes",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.SearchByAttributes",
                            Model: null,
                            Prefix: Prefix));
        }

    }
}
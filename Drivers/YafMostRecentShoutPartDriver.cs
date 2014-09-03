using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class YafMostRecentShoutPartDriver : ContentPartDriver<YafMostRecentShoutPart>
    {
        private readonly IYafMostRecentShoutService _yafMostRecentShoutService;

        protected override string Prefix { get { return "Globalcaching.YafMostRecentShoutPart"; } }

        public YafMostRecentShoutPartDriver(IYafMostRecentShoutService yafMostRecentShoutService)
        {
            _yafMostRecentShoutService = yafMostRecentShoutService;
        }

        protected override DriverResult Display(YafMostRecentShoutPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Yaf_MostRecentShout",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Yaf.MostRecentShout",
                            Model: _yafMostRecentShoutService.GetMostRecentShout(),
                            Prefix: Prefix));
        }

    }
}
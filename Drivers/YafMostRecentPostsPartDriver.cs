using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class YafMostRecentPostsPartDriver : ContentPartDriver<YafMostRecentPostsPart>
    {
        private readonly IYafMostRecentPostsService _yafMostRecentPostsService;

        protected override string Prefix { get { return "Globalcaching.YafMostRecentPostsPart"; } }

        public YafMostRecentPostsPartDriver(IYafMostRecentPostsService yafMostRecentPostsService)
        {
            _yafMostRecentPostsService = yafMostRecentPostsService;
        }

        protected override DriverResult Display(YafMostRecentPostsPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Yaf_MostRecentPosts",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Yaf.MostRecentPosts",
                            Model: _yafMostRecentPostsService.GetMostRecentPosts(),
                            Prefix: Prefix));
        }

    }
}
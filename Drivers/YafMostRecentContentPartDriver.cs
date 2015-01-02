using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class YafMostRecentContentPartDriver : ContentPartDriver<YafMostRecentContentPart>
    {
        private readonly IYafMostRecentPostsService _yafMostRecentPostsService;
        private readonly IYafMostRecentShoutService _yafMostRecentShoutService;

        protected override string Prefix { get { return "Globalcaching.YafMostRecentContentPart"; } }

        public YafMostRecentContentPartDriver(IYafMostRecentPostsService yafMostRecentPostsService,
            IYafMostRecentShoutService yafMostRecentShoutService)
        {
            _yafMostRecentPostsService = yafMostRecentPostsService;
            _yafMostRecentShoutService = yafMostRecentShoutService;
        }

        protected override DriverResult Display(YafMostRecentContentPart part, string displayType, dynamic shapeHelper)
        {
            YafMostRecentContentModel m = new YafMostRecentContentModel();
            m.YafPosts = _yafMostRecentPostsService.GetMostRecentPosts();
            m.YafShouts = _yafMostRecentShoutService.GetMostRecentShout();
            return ContentShape("Yaf_MostRecentContent",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Yaf.MostRecentContent",
                            Model: m,
                            Prefix: Prefix));
        }

    }
}
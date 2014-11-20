using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GlobalcachingMessagesDriver : ContentPartDriver<GlobalcachingMessagesPart>
    {
        private IGlobalcachingMessagesService _globalcachingMessagesService;

        public GlobalcachingMessagesDriver(IGlobalcachingMessagesService globalcachingMessagesService)
        {
            _globalcachingMessagesService = globalcachingMessagesService;
        }

        protected override DriverResult Display(GlobalcachingMessagesPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_GlobalcachingMessages",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GlobalcachingMessages",
                            Model: _globalcachingMessagesService.GetMessages(),
                            Prefix: Prefix));
        }

    }
}
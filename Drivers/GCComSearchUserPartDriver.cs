using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GCComSearchUserPartDriver : ContentPartDriver<GCComSearchUserPart>
    {
        private readonly IGCComSearchUserService _gcComSearchUserService;

        protected override string Prefix { get { return "Globalcaching.GCComSearchUserPart"; } }

        public GCComSearchUserPartDriver(IGCComSearchUserService gcComSearchUserService)
        {
            _gcComSearchUserService = gcComSearchUserService;
        }

        protected override DriverResult Display(GCComSearchUserPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_GCComSearchUser",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GCComSearchUser",
                            Model: null,
                            Prefix: Prefix));
        }
    }
}
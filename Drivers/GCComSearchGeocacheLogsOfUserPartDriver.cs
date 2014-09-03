using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GCComSearchGeocacheLogsOfUserPartDriver : ContentPartDriver<GCComSearchLogsPart>
    {
        private readonly IGCComSearchGeocacheLogsService _gcComSearchGeocacheLogsService;

        protected override string Prefix { get { return "Globalcaching.GCComSearchLogsPart"; } }

        public GCComSearchGeocacheLogsOfUserPartDriver(IGCComSearchGeocacheLogsService gcComSearchGeocacheLogsService)
        {
            _gcComSearchGeocacheLogsService = gcComSearchGeocacheLogsService;
        }

        protected override DriverResult Display(GCComSearchLogsPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_GCComSearchGeocacheLogsUser",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GCComSearchGeocacheLogsUser",
                            Model: null,
                            Prefix: Prefix));
        }
    }
}
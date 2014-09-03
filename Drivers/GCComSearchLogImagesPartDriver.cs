using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class GCComSearchLogImagesPartDriver : ContentPartDriver<GCComSearchLogImagesPart>
    {
        private readonly IGCComSearchLogImagesService _gcComSearchLogImagesService;

        protected override string Prefix { get { return ""; } }

        public GCComSearchLogImagesPartDriver(IGCComSearchLogImagesService gcComSearchLogImagesService)
        {
            _gcComSearchLogImagesService = gcComSearchLogImagesService;
        }

        protected override DriverResult Display(GCComSearchLogImagesPart part, string displayType, dynamic shapeHelper)
        {
            GCComSearchLogImagesFilter filter = new GCComSearchLogImagesFilter();
            filter.StartDate = DateTime.Now.AddDays(-1);
            filter.EndDate = DateTime.Now;
            filter.CountryID = 0;
            var m = _gcComSearchLogImagesService.GetLogImages(filter, 1, 30);
            return ContentShape("Parts_GCComSearchLogImages",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.GCComSearchLogImages",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class FavoriteGeocachesPartPartDriver : ContentPartDriver<FavoriteGeocachesPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IFavoriteGeocachesService _favoriteGeocachesService;

        public FavoriteGeocachesPartPartDriver(IFavoriteGeocachesService favoriteGeocachesService)
        {
            _favoriteGeocachesService = favoriteGeocachesService;
        }

        protected override DriverResult Display(FavoriteGeocachesPart part, string displayType, dynamic shapeHelper)
        {
            FavoriteGeocacheFilter filter = new FavoriteGeocacheFilter();
            filter.CountryId = 141;
            filter.MinDaysOnline = 0;
            filter.MinFavorites = 5;
            filter.MinFoundCount = 1;
            filter.SortOn = 0;
            var m = _favoriteGeocachesService.GetFavoriteGeocaches(1, 50, filter);
            return ContentShape("Parts_FavoriteGeocaches",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.FavoriteGeocaches",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
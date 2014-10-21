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
    public class FavoriteGeocachersPartPartDriver : ContentPartDriver<FavoriteGeocachersPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IFavoriteGeocachersService _favoriteGeocachersService;

        public FavoriteGeocachersPartPartDriver(IFavoriteGeocachersService favoriteGeocachersService)
        {
            _favoriteGeocachersService = favoriteGeocachersService;
        }

        protected override DriverResult Display(FavoriteGeocachersPart part, string displayType, dynamic shapeHelper)
        {
            FavoriteGeocacherFilter filter = new FavoriteGeocacherFilter();
            filter.CountryId = 141;
            filter.CacheMinDaysOnline = 0;
            filter.CacheMinFavorites = 0;
            filter.CacheMinFoundCount = 0;
            filter.TotalMinDaysOnline = 0;
            filter.TotalMinFavorites = 1;
            filter.TotalMinFoundCount = 0;
            filter.SortOn = 0;
            var m = _favoriteGeocachersService.GetFavoriteGeocachers(1, 50, filter);
            return ContentShape("Parts_FavoriteGeocachers",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.FavoriteGeocachers",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
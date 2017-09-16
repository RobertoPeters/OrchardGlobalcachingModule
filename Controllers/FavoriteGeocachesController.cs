using Globalcaching.Services;
using Globalcaching.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class FavoriteGeocachesController: Controller
    {
        private readonly IFavoriteGeocachesService _favoriteGeocachesService;

        public FavoriteGeocachesController(IFavoriteGeocachesService favoriteGeocachesService)
        {
            _favoriteGeocachesService = favoriteGeocachesService;
        }

        public ActionResult GetFavoriteGeocaches(int page, int pageSize, int countryId, int minFav, int minFounds, int minDays, int sortOn)
        {
            var filter = new FavoriteGeocacheFilter();
            filter.CountryId = countryId;
            filter.MinDaysOnline = minDays;
            filter.MinFavorites = minFav;
            filter.MinFoundCount = minFounds;
            filter.SortOn = sortOn;
            return Json(_favoriteGeocachesService.GetFavoriteGeocaches(page, pageSize, filter));
        }
    }
}
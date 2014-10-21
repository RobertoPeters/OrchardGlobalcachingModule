using Globalcaching.Services;
using Globalcaching.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class FavoriteGeocachersController: Controller
    {
        private readonly IFavoriteGeocachersService _favoriteGeocachersService;

        public FavoriteGeocachersController(IFavoriteGeocachersService favoriteGeocachersService)
        {
            _favoriteGeocachersService = favoriteGeocachersService;
        }

        public ActionResult GetFavoriteGeocachers(int page, int pageSize, int countryId, int minFav, int minFounds, int minDays, int tminFav, int tminFounds, int tminDays, string usrName, int sortOn)
        {
            var filter = new FavoriteGeocacherFilter();
            filter.CountryId = countryId;
            filter.CacheMinDaysOnline = minDays;
            filter.CacheMinFavorites = minFav;
            filter.CacheMinFoundCount = minFounds;
            filter.TotalMinDaysOnline = tminDays;
            filter.TotalMinFavorites = tminFav;
            filter.TotalMinFoundCount = tminFounds;
            filter.UserName = usrName;
            filter.SortOn = sortOn;
            return Json(_favoriteGeocachersService.GetFavoriteGeocachers(page, pageSize, filter));
        }
    }
}
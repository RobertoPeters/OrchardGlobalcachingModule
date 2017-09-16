using Globalcaching.Services;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class BookmarksController : Controller
    {
        private readonly IGCComSearchUserService _gcComSearchUserService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IBookmarksService _bookmarkService;

        public BookmarksController(IGCComSearchUserService gcComSearchUserService,
            IGCEuUserSettingsService gcEuUserSettingsService,
            IBookmarksService bookmarkService)
        {
            _gcComSearchUserService = gcComSearchUserService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _bookmarkService = bookmarkService;
        }

        [HttpPost]
        public ActionResult GetBookmarks(int page, int pageSize, string ListName, string UserName, int NumberOfItems, int NumberOfKnownItems)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                return Json(_bookmarkService.GetBookmarks(page, pageSize, ListName, UserName, NumberOfItems, NumberOfKnownItems));
            }
            else
            {
                return null;
            }
        }

    }
}
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
    public class BookmarksPartPartDriver : ContentPartDriver<BookmarksPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IBookmarksService _bookmarksService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public BookmarksPartPartDriver(IBookmarksService bookmarksService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _bookmarksService = bookmarksService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(BookmarksPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.IsDonator)
            {
                var m = _bookmarksService.GetBookmarks(1, 50, "", "", 1, 1);
                return ContentShape("Parts_Bookmarks",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.Bookmarks",
                                Model: m,
                                Prefix: Prefix));
            }
            else
            {
                return ContentShape("Parts_ForDonatorsOnly",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.ForDonatorsOnly",
                                Model: null,
                                Prefix: Prefix));
            }
        }
    }
}
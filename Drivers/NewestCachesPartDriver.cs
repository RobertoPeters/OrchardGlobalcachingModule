using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class NewestCachesPartDriver : ContentPartDriver<NewestCachesPart>
    {
        private IGeocacheSearchFilterService _geocacheSearchFilterService;

        public NewestCachesPartDriver(IGeocacheSearchFilterService geocacheSearchFilterService)
        {
            _geocacheSearchFilterService = geocacheSearchFilterService;
        }

        protected override DriverResult Display(NewestCachesPart part, string displayType, dynamic shapeHelper)
        {
            NewestCachesModel m = new NewestCachesModel();
            GeocacheSearchFilter filter = new GeocacheSearchFilter();
            filter.OrderBy = (int)GeocacheSearchFilterOrderOnItem.PublicationDate;
            filter.OrderByDirection = -1;
            filter.MaxResult = 10;
            filter.Parel = true;
            m.Geocaches = _geocacheSearchFilterService.GetGeocaches(filter);
            return ContentShape("Parts_NewestCaches",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.NewestCaches",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
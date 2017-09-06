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
    public class ShopUserProductPartDriver : ContentPartDriver<ShopUserProductPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IGCEuUserSettingsService _userSettingsService;
        private readonly IShopService _shopService;

        public ShopUserProductPartDriver(IGCEuUserSettingsService userSettingsService,
            IShopService shopService)
        {
            _userSettingsService = userSettingsService;
            _shopService = shopService;
        }

        protected override DriverResult Display(ShopUserProductPart part, string displayType, dynamic shapeHelper)
        {
            var usr = _userSettingsService.GetSettings();
            if (usr != null && usr.IsDonator)
            {
                var m = _shopService.GetShopUserProductModel(usr.YafUserID);
                return ContentShape("Parts_ShopUserProduct",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.ShopUserProduct",
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
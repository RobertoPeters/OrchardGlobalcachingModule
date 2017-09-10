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
    public class ShopContactUserProductPartDriver : ContentPartDriver<ShopContactUserProductPart>
    {
        private readonly IShopService _shopService;
        private readonly IWorkContextAccessor _workContextAccessor;

        protected override string Prefix { get { return ""; } }

        public ShopContactUserProductPartDriver(IShopService shopService,
            IWorkContextAccessor workContextAccessor)
        {
            _shopService = shopService;
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        protected override DriverResult Display(ShopContactUserProductPart part, string displayType, dynamic shapeHelper)
        {
            string code = HttpContext.Request.QueryString["code"];
            return ContentShape("Parts_ShopContactUserProduct",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.ShopContactUserProduct",
                            Model: _shopService.GetShopContactUserProductModel(code),
                            Prefix: Prefix));
        }
    }
}
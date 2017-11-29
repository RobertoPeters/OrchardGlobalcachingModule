using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class ShopArticleDriver : ContentPartDriver<ShopArticlePart>
    {
        private readonly IShopService _shopService;

        public ShopArticleDriver(IShopService shopService)
        {
            _shopService = shopService;
        }

        protected override DriverResult Display(ShopArticlePart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_ShopArticle",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.ShopArticle",
                            Model: _shopService.GetShopArticle(part),
                            Prefix: Prefix));
        }

        protected override DriverResult Editor(ShopArticlePart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_ShopArticle_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ShopArticle",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(ShopArticlePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }

}
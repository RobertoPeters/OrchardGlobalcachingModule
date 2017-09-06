using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class ShopUserProductModelItem: GcEuProducts
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
    }

    public class ShopUserProductModel
    {
        public List<ShopCategory> AllCategories { get; set; }
        public List<ShopUserProductModelItem> Products { get; set; }
        public ShopUserProductModelItem ActiveProduct { get; set; }
        public int MaxAllowedProducts { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class ShopArticleModel
    {
        [PetaPoco.Ignore]
        public int ProductId { get; set; }
        [PetaPoco.Ignore]
        public string Comment { get; set; }

        public string Name { get; set; }
        public string Sku { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public double Price { get; set; }
        public string Url { get; set; }
    }
}
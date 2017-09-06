using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class ShopAdminModel
    {
        public DateTime? TokenFromDate { get; set; }
        public DateTime? TokenExpires { get; set; }
        public ShopCategory MasterCategory { get; set; }
        public List<ShopCategory> AllCategories { get; set; }       
    }
}
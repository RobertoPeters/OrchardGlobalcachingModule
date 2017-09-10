using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class ShopContactUserProductModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string SellerName { get; set; }
        public string BuyerName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Ongeldig email adres")]
        public string BuyerEmail { get; set; }

        [Required]
        [DataType(DataType.MultilineText, ErrorMessage = "Plaats jouw vraag/opmerking")]
        public string BuyerComment { get; set; }
    }
}
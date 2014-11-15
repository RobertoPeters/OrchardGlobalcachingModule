using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class EditUserSettingsModel
    {
        //[Required]
        public bool ShowGeocachesOnGlobal { get; set; }
        public string Homelocation { get; set; }
        public int DefaultCountryCode { get; set; }
        public string ReturnUrl { get; set; }
        public string MarkLogTextColor1 { get; set; }
        public string MarkLogTextColor2 { get; set; }
        public string MarkLogTextColor3 { get; set; }
    }
}
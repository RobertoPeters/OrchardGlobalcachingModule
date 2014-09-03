using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class GCComQuickGeocacheSearch
    {
        public GCEuUserSettings Settings { get; set; }
        public string NameContainsWord { get; set; }
        public string Location { get; set; }
        public string Radius { get; set; }
        public string HiddenBy { get; set; }
        public int CountryID { get; set; }
    }
}
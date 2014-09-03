using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class GeocacheSearchFilter
    {
        public long Page { get; set; }
        public long PageSize { get; set; }

        public int MaxResult { get; set; }
        public long? OwnerID { get; set; }
        public int? CountryID { get; set; }
        public double? HomeLat { get; set; }
        public double? HomeLon { get; set; }
        public double? CenterLat { get; set; }
        public double? CenterLon { get; set; }
        public double? Radius { get; set; }
        public string NameContainsWord { get; set; }
        public string OwnerName { get; set; }
        public int? OrderBy { get; set; }
        public int? OrderByDirection { get; set; }
        public bool? MacroResult { get; set; }
    }
}
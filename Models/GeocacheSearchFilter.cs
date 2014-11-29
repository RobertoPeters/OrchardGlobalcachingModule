using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public enum GeocacheSearchFilterOrderOnItem : int
    {
        DistanceFromHome = 0,
        HiddenDate = 1,
        PublicationDate = 2,
        Distance = 3,
        MostRecentFoundDate = 4,
        Favorites = 5,
        FavoritesPercentage = 6,
        Founds = 7,
        GeocacheType = 8,
        Code = 9,
        Difficulty = 10,
        Terrain = 11,
        Name = 12,
    }

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
        public string NameSeriesMatch { get; set; }
        public bool? Parel { get; set; }
    }
}
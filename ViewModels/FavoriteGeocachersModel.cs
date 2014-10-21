using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class FavoriteGeocacherFilter
    {
        public int CountryId { get; set; }
        public int CacheMinDaysOnline { get; set; }
        public int CacheMinFavorites { get; set; }
        public int CacheMinFoundCount { get; set; }
        public int TotalMinDaysOnline { get; set; }
        public int TotalMinFavorites { get; set; }
        public int TotalMinFoundCount { get; set; }
        public string UserName { get; set; }
        public int SortOn { get; set; } //0=#, 1=%
    }

    public class FavoriteGeocacherInfo
    {
        public long? OwnerId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int FoundCount { get; set; }
        public int CacheCount { get; set; }
        public int FavoriteCount { get; set; }
        public int DaysOnline { get; set; }

        [PetaPoco.Ignore]
        public double FavPer100Found { get; set; }
        [PetaPoco.Ignore]
        public string DirectionIcon { get; set; }
        [PetaPoco.Ignore]
        public double DistanceFromHome { get; set; }

        //gccomuser
        public string UserName { get; set; }
        public Guid PublicGuid { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class FavoriteGeocachersModel
    {
        public FavoriteGeocacherFilter Filter { get; set; }
        public List<FavoriteGeocacherInfo> FavoriteGeocachers { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
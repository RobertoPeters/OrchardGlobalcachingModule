using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class FavoriteGeocacheFilter
    {
        public int CountryId { get; set; }
        public int MinDaysOnline { get; set; }
        public int MinFavorites { get; set; }
        public int MinFoundCount { get; set; }
        public int SortOn { get; set; } //0=#, 1=%
    }

    public class FavoriteGeocacheInfo
    {
        //gccomgeocache
        public long ID { get; set; }
        public bool? Archived { get; set; }
        public bool? Available { get; set; }
        public long GeocacheTypeId { get; set; }
        public string Code { get; set; }
        public long ContainerTypeId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Name { get; set; }
        public long? OwnerId { get; set; }
        public int? FavoritePoints { get; set; }
        public string Url { get; set; }

        //gccomuser
        public string UserName { get; set; }
        public Guid PublicGuid { get; set; }
        public string AvatarUrl { get; set; }

        //GCEuGeocache
        public int FoundCount { get; set; }

        //computed
        public int DaysOnline { get; set; }
        public double FavPer100Found { get; set; }

        [PetaPoco.Ignore]
        public string DirectionIcon { get; set; }
        [PetaPoco.Ignore]
        public double DistanceFromHome { get; set; }
        [PetaPoco.Ignore]
        public bool Found { get; set; }
        [PetaPoco.Ignore]
        public bool Own { get; set; }
        [PetaPoco.Ignore]
        public string ContainerIcon { get; set; }

    }

    public class FavoriteGeocachesModel
    {
        public FavoriteGeocacheFilter Filter { get; set; }
        public List<FavoriteGeocacheInfo> FavoriteGeocaches { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
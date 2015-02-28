using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class GeocacheListItem
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public bool? Archived { get; set; }
        public bool? Available { get; set; }
        public long GeocacheTypeId { get; set; }
        public double Difficulty { get; set; }
        public double Terrain { get; set; }
        public long ContainerTypeId { get; set; }
        public string Municipality { get; set; }
        public double? Distance { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceFromHome { get; set; }
        public long? OwnerId { get; set; }
        public DateTime UTCPlaceDate { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string City { get; set; }
        public string UserName { get; set; }
        public Guid PublicGuid { get; set; }
        public int? FavoritePoints { get; set; }
        public int FoundCount { get; set; }
        public DateTime? PublishedAtDate { get; set; }
        public DateTime? MostRecentFoundDate { get; set; }
        public double? FavPer100Found { get; set; }
        
        [PetaPoco.Ignore]
        public string DirectionIcon { get; set; }
        [PetaPoco.Ignore]
        public bool Found { get; set; }
        [PetaPoco.Ignore]
        public bool Own { get; set; }
        [PetaPoco.Ignore]
        public string ContainerIcon { get; set; }
    }

    public class GeocacheSearchResult
    {
        public GeocacheSearchFilter Filter { get; set; }
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public long CurrentPage { get; set; }
        public bool CanDownload { get; set; }
        public GeocacheListItem[] Items { get; set; }
    }
}
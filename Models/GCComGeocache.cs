using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComGeocache")]
    public class GCComGeocache
    {
        public long ID { get; set; }
        public bool? Archived { get; set; }
        public bool? Available { get; set; }
        public long GeocacheTypeId { get; set; }
        public string Code { get; set; }
        public long ContainerTypeId { get; set; }
        public bool IsContainer { get; set; }
        public string Country { get; set; }
        public int CountryID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdate { get; set; }
        public DateTime? DateLastVisited { get; set; }
        public double Difficulty { get; set; }
        public string EncodedHints { get; set; }
        public int? FavoritePoints { get; set; }
        public Guid GUID { get; set; }
        public int ImageCount { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsPremium { get; set; }
        public double? Latitude { get; set; }
        public string LongDescription { get; set; }
        public bool LongDescriptionIsHtml { get; set; }
        public double? Longitude { get; set; }
        public string Name { get; set; }
        public long? OwnerId { get; set; }
        public string PlacedBy { get; set; }
        public string ShortDescription { get; set; }
        public bool ShortDescriptionIsHtml { get; set; }
        public string State { get; set; }
        public int StateID { get; set; }
        public double Terrain { get; set; }
        public int TrackableCount { get; set; }
        public string Url { get; set; }
        public DateTime UTCPlaceDate { get; set; }
    }
}
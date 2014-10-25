using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class LogImageStatsFilter
    {
        public int CountryId { get; set; }
        public int CacheTypeId { get; set; }
        public int MinImageCount { get; set; }
        public int MinFoundCount { get; set; }
        public int MinDaysOnline { get; set; }
        public int SortOn { get; set; } //0=#, 1=%
    }

    public class LogImageInfo
    {
        public string ThumbUrl { get; set; }
        public string Url { get; set; }
    }

    public class LogImageStatsInfo
    {
        public long ID { get; set; }
        public long? OwnerId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int FoundCount { get; set; }
        public int DaysOnline { get; set; }
        public long GeocacheTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? LogImageCount { get; set; }
        public double? LogImagePer100Found { get; set; }

        //gccomuser
        public string UserName { get; set; }
        public Guid PublicGuid { get; set; }

        [PetaPoco.Ignore]
        public bool Found { get; set; }
        [PetaPoco.Ignore]
        public bool Own { get; set; }
        [PetaPoco.Ignore]
        public string DirectionIcon { get; set; }
        [PetaPoco.Ignore]
        public double DistanceFromHome { get; set; }
        [PetaPoco.Ignore]
        public List<LogImageInfo> LogImages { get; set; }
    }

    public class LogImageStatsModel
    {
        public LogImageStatsFilter Filter { get; set; }
        public List<LogImageStatsInfo> Geocaches { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
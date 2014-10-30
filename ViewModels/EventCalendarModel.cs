using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheEventInfo
    {
        public int ID { get; set; }
        public DateTime UTCPlaceDate { get; set; }
        public string Url { get; set; }
        public long? OwnerId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long GeocacheTypeId { get; set; }

        //gccomuser
        public string UserName { get; set; }
        public Guid PublicGuid { get; set; }
        public string AvatarUrl { get; set; }

        [PetaPoco.Ignore]
        public string DirectionIcon { get; set; }
        [PetaPoco.Ignore]
        public double DistanceFromHome { get; set; }
    }

    public class EventCalendarModel
    {
        public List<GeocacheEventInfo> Events { get; set; }
    }
}
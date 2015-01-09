using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComTrackable")]
    public class GCComTrackable
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public bool? AllowedToBeCollected { get; set; }
        public long BugTypeID { get; set; }
        public string CurrentGeocacheCode { get; set; }
        public string CurrentGoal { get; set; }
        public long? CurrentOwnerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string Name { get; set; }
        public long? OriginalOwnerId { get; set; }
        public string TBTypeName { get; set; }
        public string TBTypeNameSingular { get; set; }
        public string TrackingCode { get; set; }
        public string Url { get; set; }
        public long? UserCount { get; set; }
        public long WptTypeID { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComTrackableLog")]
    public class GCComTrackableLog
    {
        public int ID { get; set; }
        public long TrackableID { get; set; }
        public string Code { get; set; }
        public int? CacheID { get; set; }
        public long? LoggedBy { get; set; }
        public Guid LogGuid { get; set; }
        public bool LogIsEncoded { get; set; }
        public string LogText { get; set; }
        public long? WptLogTypeId { get; set; }
        public double? UpdatedLatitude { get; set; }
        public double? UpdatedLongitude { get; set; }
        public string Url { get; set; }
        public DateTime UTCCreateDate { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
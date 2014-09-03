using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComGeocacheLog")]
    public class GCComGeocacheLog
    {
        public long ID { get; set; }
        public long GeocacheID { get; set; }
        public string CacheCode { get; set; }
        public string Code { get; set; }
        public long? FinderId { get; set; }
        public Guid Guid { get; set; }
        public bool LogIsEncoded { get; set; }
        public string LogText { get; set; }
        public long WptLogTypeId { get; set; }
        public double? UpdatedLatitude { get; set; }
        public double? UpdatedLongitude { get; set; }
        public string Url { get; set; }
        public DateTime UTCCreateDate { get; set; }
        public DateTime VisitDate { get; set; }
    }
    public class GCComGeocacheLogEx : GCComGeocacheLog
    {
        public long GeocacheTypeId { get; set; }
        public string Name { get; set; }
        public string GeocacheUrl { get; set; }
    }
    public class GCComGeocacheLogOfUserSearchResult
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public long CurrentPage { get; set; }
        public GCComUser User { get; set; }
        public GCComGeocacheLogEx[] Logs { get; set; }
    }

}
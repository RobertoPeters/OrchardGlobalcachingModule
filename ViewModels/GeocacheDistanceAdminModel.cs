using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheDistanceAdminInfo
    {
        //GCEuGeocache
        public long ID { get; set; }
        public DateTime? PublishedAtDate { get; set; }
        public int? DistanceHandledBy { get; set; }

        //GCComGeocache
        public string Code { get; set; }
        public string Name { get; set; }

        //GCComGeocacheType
        public string GeocacheTypeName { get; set; }
    }

    public class GeocacheDistanceAdminModel
    {
        public List<GeocacheDistanceAdminInfo> Geocaches { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class FTFAdminInfo
    {
        //GCEuGeocache
        public long ID { get; set; }
        public long? FTFUserID { get; set; }
        public long? STFUserID { get; set; }
        public long? TTFUserID { get; set; }
        public int FoundCount { get; set; }
        public int FTFFoundCount { get; set; }
        public DateTime? PublishedAtDate { get; set; }

        //GCComGeocache
        public string Code { get; set; }

        //GCComGeocacheType
        public string GeocacheTypeName { get; set; }

        [PetaPoco.Ignore]
        public int NewLogs { get; set; }
    }

    public class FTFAdminModel
    {
        public List<FTFAdminInfo> Geocaches { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
        public int QueueLength { get; set; }
    }
}
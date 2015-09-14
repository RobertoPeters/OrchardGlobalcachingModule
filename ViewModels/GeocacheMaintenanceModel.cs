using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheMaintenanceGeocacheInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ID { get; set; }
        public int ContainerTypeId { get; set; }
        public int GeocacheTypeId { get; set; }
        public string Url { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int FoundCount { get; set; }
        public DateTime? LastOwnerMaintenance { get; set; }
        public int CountMaintenance { get; set; }
        public int LogsMaintenance { get; set; }
    }

    public class GeocacheMaintenanceModel
    {
        public List<GeocacheMaintenanceGeocacheInfo> Geocaches { get; set; }
        public string UserName { get; set; }
    }
}
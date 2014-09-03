using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class GCEuGeocacheFilterMacroPart : ContentPart
    {
    }

    [PetaPoco.TableName("GCEuGeocacheFilterMacro")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuGeocacheFilterMacro
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string RawMacro { get; set; }
        public string CleanMacro { get; set; }
        public int? TotalSteps { get; set; }
        public int? ProcessedSteps { get; set; }
        public string ProcessInfo { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishedTime { get; set; }
    }
}
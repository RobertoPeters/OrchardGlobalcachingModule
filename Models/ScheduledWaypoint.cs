using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("ScheduledWaypoint")]
    public class ScheduledWaypoint
    {
        public string Code { get; set; }
        public bool FullRefresh { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
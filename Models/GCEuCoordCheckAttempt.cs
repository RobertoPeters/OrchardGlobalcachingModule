using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuCoordCheckAttempt")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuCoordCheckAttempt
    {
        public int ID { get; set; }
        public string Waypoint { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string VisitorID { get; set; }
        public DateTime AttemptAt { get; set; }

        [PetaPoco.Ignore]
        public string Coordinates { get; set; }
    }
}
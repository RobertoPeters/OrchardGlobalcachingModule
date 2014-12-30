﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuCoordCheckCode")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuCoordCheckCode
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int Radius { get; set; }
        public bool NotifyOnSuccess { get; set; }
        public bool NotifyOnFailure { get; set; }
        public string Code { get; set; }

        [PetaPoco.Ignore]
        public string Coordinates { get; set; }
    }
}
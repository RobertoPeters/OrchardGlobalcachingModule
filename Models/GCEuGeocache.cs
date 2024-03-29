﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tucson.Geocaching.WCF.API.Geocaching1.Types;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuGeocache")]
    public class GCEuGeocache
    {
        public long ID { get; set; }
        public long? FTFUserID { get; set; }
        public long? STFUserID { get; set; }
        public long? TTFUserID { get; set; }
        public string Municipality { get; set; }
        public string City { get; set; }
        public int FoundCount { get; set; }
        public double? Distance { get; set; }
        public bool DistanceChecked { get; set; }
        public bool FTFCompleted { get; set; }
        public DateTime? FTFAtDate { get; set; }
        public DateTime? STFAtDate { get; set; }
        public DateTime? TTFAtDate { get; set; }
        public DateTime? PublishedAtDate { get; set; }
        public double? FavPer100Found { get; set; }
        public int? LogImageCount { get; set; }
        public double? LogImagePer100Found { get; set; }
        public int FTFFoundCount { get; set; }
        public DateTime? MostRecentFoundDate { get; set; }
        public DateTime? MostRecentArchivedDate { get; set; }
        public int PMFoundCount { get; set; }
        public int? DistanceHandledBy { get; set; }
    }
}
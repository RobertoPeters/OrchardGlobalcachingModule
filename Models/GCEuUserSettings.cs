using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuUserSettings")]
    public class GCEuUserSettings
    {
        public GCEuUserSettings()
        {
            ExpirationTime = DateTime.Now.AddMinutes(30);
        }

        public int YafUserID { get; set; }
        public long? GCComUserID { get; set; }
        public string LiveAPIToken { get; set; }
        public bool? ShowGeocachesOnGlobal { get; set; }
        public double? HomelocationLat { get; set; }
        public double? HomelocationLon { get; set; }
        public int? DefaultCountryCode { get; set; }
        public string MarkLogTextColor1 { get; set; }
        public string MarkLogTextColor2 { get; set; }
        public string MarkLogTextColor3 { get; set; }
        public int? SortGeocachesBy { get; set; }
        public int? SortGeocachesDirection { get; set; }
        public int? NewestCachesMode { get; set; }

        [PetaPoco.Ignore]
        public bool IsPM { get; set; }
        [PetaPoco.Ignore]
        public bool IsDonator { get; set; }
        [PetaPoco.Ignore]
        public DateTime ExpirationTime { get; set; }
    }
}
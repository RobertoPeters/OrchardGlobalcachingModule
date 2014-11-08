using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuFTFStats")]
    public class GCEuFTFStats
    {
        public long UserID { get; set; }
        public int? Jaar { get; set; }
        public int FTFCount { get; set; }
        public int STFCount { get; set; }
        public int TTFCount { get; set; }
        public int Position { get; set; }
        public int PositionPoints { get; set; }
    }
}
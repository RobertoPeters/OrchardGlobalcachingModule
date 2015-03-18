using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuLiveAPIHelpers")]
    public class GCEuLiveAPIHelpers
    {
        public int YafUserID { get; set; }
        public long GCComUserID { get; set; }
        public string LiveAPIToken { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GcComAccounts")]
    [PetaPoco.PrimaryKey("ID")]
    public class GcComAccounts
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public bool Enabled { get; set; }
        public int? CachesLeft { get; set; }
        public int? CurrentCacheCount { get; set; }
        public DateTime? LimitsUpdatedAt { get; set; }

        [PetaPoco.Ignore]
        public bool PM { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuServiceCall")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuServiceCall
    {
        public long ID { get; set; }
        public DateTime CalledAt { get; set; }
        public string IPAddress { get; set; }
        public string ServiceName { get; set; }
        public bool Token { get; set; }
        public bool Credentials { get; set; }
        public int? UserID { get; set; }
        public long? GCComUserID { get; set; }
    }
}
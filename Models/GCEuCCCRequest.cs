using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuCCCRequest")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuCCCRequest
    {
        public int ID { get; set; }
        public DateTime RequestAt { get; set; }
        public string UserName { get; set; }
        public long? GCComUserID { get; set; }
        public bool ValidPassword { get; set; }
        public string GCCode { get; set; }
        public bool MemberCCC { get; set; }
        public int ResultCount { get; set; }
    }
}
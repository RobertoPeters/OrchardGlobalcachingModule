using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuApprovedCode")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuApprovedCode
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Code { get; set; }
        public string Comment { get; set; }
        public DateTime ApprovedAt { get; set; }
        public DateTime LastEditAt { get; set; }
    }
}
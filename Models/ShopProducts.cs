using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GcEuProducts")]
    [PetaPoco.PrimaryKey("Id")]
    public class GcEuProducts
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
    }
}
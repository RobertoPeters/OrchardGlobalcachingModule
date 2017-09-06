using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GcEuAccess")]
    [PetaPoco.PrimaryKey("ID")]
    public class ShopAccess
    {
        public int ID { get; set; }
        public DateTime Updated { get; set; }
        public string Token { get; set; }
        public string TokenType { get; set; }
        public long? ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public int? MasterCategoryId { get; set; }       
    }
}
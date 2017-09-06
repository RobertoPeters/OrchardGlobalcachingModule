using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("Category")]
    [PetaPoco.PrimaryKey("Id")]
    public class ShopCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public bool Deleted { get; set; }

        [PetaPoco.Ignore]
        public string FullPath { get; set; }

        [PetaPoco.Ignore]
        public bool IsSubOfMaster { get; set; }
    }
}
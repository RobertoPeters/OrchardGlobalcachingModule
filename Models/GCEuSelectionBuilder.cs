using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuSelectionBuilder")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuSelectionBuilder
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Graph { get; set; }
        public bool IsPublic { get; set; }
        public string Comment { get; set; }
        public DateTime RecentEdit { get; set; }
    }
}
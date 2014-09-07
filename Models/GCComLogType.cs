using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComLogType")]
    public class GCComLogType
    {
        public long ID { get; set; }
        public bool AdminActionable { get; set; }
        public string ImageName { get; set; }
        public string ImageURL { get; set; }
        public bool OwnerActionable { get; set; }
        public string WptLogTypeName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComGeocacheType")]
    public class GCComGeocacheType
    {
        public long ID { get; set; }
        public string Description { get; set; }
        public string GeocacheTypeName { get; set; }
        public string ImageURL { get; set; }
        public bool IsContainer { get; set; }
    }
}
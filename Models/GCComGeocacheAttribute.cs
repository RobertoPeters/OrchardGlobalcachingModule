using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComGeocacheAttribute")]
    public class GCComGeocacheAttribute
    {
        public long GeocacheID { get; set; }
        public int AttributeTypeID { get; set; }
        public bool IsOn { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComBookmarkContent")]
    public class GCComBookmarkContent
    {
        public long GCComBookmarkListID { get; set; }
        public long GCComGeocacheID { get; set; }
    }
}
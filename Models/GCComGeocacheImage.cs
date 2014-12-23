using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComGeocacheImage")]
    public class GCComGeocacheImage
    {
        public long GeocacheID { get; set; }
        public string Description { get; set; }
        public Guid Guid { get; set; }
        public string MobileUrl { get; set; }
        public string Name { get; set; }
        public string ThumbUrl { get; set; }
        public string Url { get; set; }
    }
}
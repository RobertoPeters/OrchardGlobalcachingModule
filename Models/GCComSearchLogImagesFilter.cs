using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class GCComSearchLogImagesFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CountryID { get; set; }
    }
}
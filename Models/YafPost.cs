using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class YafPost
    {
        public string Username { get; set; }
        public string PostUrl { get; set; }
        public DateTime PostedAt { get; set; }
        public string Topic { get; set; }
    }
}
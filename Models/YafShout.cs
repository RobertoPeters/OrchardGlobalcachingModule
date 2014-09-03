using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class YafShout
    {
        public string Username { get; set; }
        public DateTime PostedAt { get; set; }
        public string Message { get; set; }
    }
}
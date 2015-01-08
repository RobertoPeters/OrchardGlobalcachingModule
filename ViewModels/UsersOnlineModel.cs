using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class UsersOnlineModel
    {
        public string Name { get; set; }
        public int UserID { get; set; }
        public string IP { get; set; }
        public DateTime LastActive { get; set; }
        public string Location { get; set; }
        public string PageParam { get; set; }
    }
}
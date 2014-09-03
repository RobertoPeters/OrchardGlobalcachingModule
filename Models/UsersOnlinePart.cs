using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class UsersOnlinePart : ContentPart
    {
    }

    public class OnlineUser
    {
        public int YafUserID { get; set; }
        public string YafUserName { get; set; }
        public bool Donator { get; set; }
        public string LastAccess { get; set; }
        public int Count { get; set; }
        public bool IsPosting { get; set; }
    }

    public class OnlineUserInfo
    {
        public int Count { get; set; }
        public List<OnlineUser> Users { get; set; }
    }
}
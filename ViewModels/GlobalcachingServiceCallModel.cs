using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GlobalcachingServiceCallItem
    {
        public DateTime CalledAt { get; set; }
        public string IPAddress { get; set; }
        public string ServiceName { get; set; }
        public bool Token { get; set; }
        public bool Credentials { get; set; }
        public string GlobalNick { get; set; }
        public string GCComNick { get; set; }
        public int MemberTypeId { get; set; }
        public bool IsDonator { get; set; }
    }

    public class GlobalcachingServiceCallModel
    {
        public List<GlobalcachingServiceCallItem> Calls { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
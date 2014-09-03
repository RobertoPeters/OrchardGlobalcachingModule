using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class CheckCCCItem
    {
        //GCEuCCCUser
        public int UserID { get; set; }
        public bool Active { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public long GCComUserID { get; set; }
        public string Telnr { get; set; }
        public bool SMS { get; set; }
        public bool PreferSMS { get; set; }
        public string TwitterUsername { get; set; }
        public int UsersHelped { get; set; }
        public string Comment { get; set; }
        public bool HideEmailAddress { get; set; }

        //yaf_User
        public string EMail { get; set; }

        //GCComUser
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public Guid PublicGuid { get; set; }
        public int? FindCount { get; set; }

        //GCComGeocacheLog
        public DateTime VisitDate { get; set; }
        public string Url { get; set; }
    }

    public class CheckCCCResult
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public long CurrentPage { get; set; }
        public string GeocacheCode { get; set; }
        public string GeocacheTitle { get; set; }
        public CheckCCCItem Owner { get; set; }
        public CheckCCCItem[] Items { get; set; }
    }
}
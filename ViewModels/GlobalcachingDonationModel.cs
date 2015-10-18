using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GlobalcachingDonationItem : GCEuDonations
    {
        public string Name { get; set; }
        public string PartnerName { get; set; }
        public bool IsDonator { get; set; }

        [PetaPoco.Ignore]
        public bool IsInWrongState { get; set; }
    }

    public class GlobalcachingDonationModel
    {
        public GlobalcachingDonationItem EditItem { get; set; }
        public List<GlobalcachingDonationItem> Items { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }

        public double TotalLastYear { get; set; }
        public double TotalThisYear { get; set; }
    }
}
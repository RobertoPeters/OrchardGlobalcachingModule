using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GCApprovedCodeModelItem : GCEuApprovedCode
    {
        public string UserName { get; set; }
    }

    public class GCApprovedCodeModel
    {
        public GCApprovedCodeModelItem EditItem { get; set; }
        public List<GCApprovedCodeModelItem> Items { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }

        public bool CanEdit { get; set; }
        public string Filter { get; set; }
    }
}
using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class CodeCheckerAttempts
    {
        public List<GCEuCodeCheckAttempt> Attempts { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }

    public class CodeCheckerMaintModel
    {
        public int OwnerID { get; set; }
        public GCEuCodeCheckCode ActiveCode { get; set; }
        public List<GCEuCodeCheckCode> Codes { get; set; }
        public CodeCheckerAttempts AttemptInfo { get; set; }
    }
}
using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class CoordCheckerAttempts
    {
        public List<GCEuCoordCheckAttempt> Attempts { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }

    public class CoordCheckerMaintModel
    {
        public GCEuCoordCheckCode ActiveCode { get; set; }
        public List<GCEuCoordCheckCode> Codes { get; set; }
        public CoordCheckerAttempts AttemptInfo { get; set; }
    }
}
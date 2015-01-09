using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class TrackableGroupMaintModel
    {
        public GCEuTrackableGroup ActiveGroup { get; set; }
        public List<GCEuTrackableGroup> Groups { get; set; }
        public List<string> TBCodes { get; set; }
    }
}
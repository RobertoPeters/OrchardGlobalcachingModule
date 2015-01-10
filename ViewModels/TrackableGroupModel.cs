using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class TrackableInfo
    {
        public GCEuTrackable GCEuTrackable { get; set; }
        public GCComTrackable GCComTrackable { get; set; }

        [PetaPoco.Ignore]
        public string DirectionIcon { get; set; }
        [PetaPoco.Ignore]
        public double Distance { get; set; }
    }

    public class TrackableGroupModel
    {
        public string UserName { get; set; }
        public GCEuTrackableGroup Group { get; set; }
        public List<TrackableInfo> Trackables { get; set; }
    }
}
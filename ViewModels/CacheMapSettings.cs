using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class CacheMapSettings
    {
        public double CenterLat { get; set; }
        public double CenterLon { get; set; }
        public int InitialZoomLevel { get; set; }

        public GeocacheSearchFilter Filter { get; set; }
    }
}
using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheLogInfo
    {
        public GeocacheLogInfo(GCComGeocacheLog log, GCComUser writer)
        {
            Log = log;
            Writer = writer;
        }

        public GCComGeocacheLog Log { get; set; }
        public GCComUser Writer { get; set; }
    }

    public class GeocacheDataModel
    {
        public bool IsFTFAdmin { get; set; }
        public bool IsDistanceAdmin { get; set; }
        public GCComGeocache GCComGeocacheData { get; set; }
        public GCEuGeocache GCEuGeocacheData { get; set; }
        public GCComUser Owner { get; set; }
        public GCComUser FTF { get; set; }
        public GCComUser STF { get; set; }
        public GCComUser TTF { get; set; }
        public List<GeocacheLogInfo> GCComGeocacheLogs { get; set; }
        public List<GCComLogType> LogTypes { get; set; }
    }
}
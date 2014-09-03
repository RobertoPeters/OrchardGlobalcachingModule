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
        public GCComGeocache GCComGeocacheData { get; set; }
        public GCEuGeocache GCEuGeocacheData { get; set; }
        public List<GeocacheLogInfo> GCComGeocacheLogs { get; set; }
    }
}
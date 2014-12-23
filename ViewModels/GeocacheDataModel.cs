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

    public class LogCountInfo
    {
        public int WptLogTypeId { get; set; }
        public int logCount { get; set; }
    }

    public class GeocacheAttributeInfo
    {
        public GeocacheAttributeInfo()
        {
        }

        public GeocacheAttributeInfo(GCComGeocacheAttribute attr, GCComAttributeType attrType)
        {
            AttributeType = attrType;
            Attribute = attr;
        }

        public GCComAttributeType AttributeType { get; set; }
        public GCComGeocacheAttribute Attribute { get; set; }
    }

    public class GeocacheDataModel
    {
        public bool IsFTFAdmin { get; set; }
        public bool IsDistanceAdmin { get; set; }
        public GCComGeocache GCComGeocacheData { get; set; }
        public GCEuGeocache GCEuGeocacheData { get; set; }
        public GCEuUserSettings UserSettings { get; set; }
        public GCComUser Owner { get; set; }
        public GCComUser FTF { get; set; }
        public GCComUser STF { get; set; }
        public GCComUser TTF { get; set; }
        public List<GeocacheLogInfo> GCComGeocacheLogs { get; set; }
        public List<GCComLogType> LogTypes { get; set; }
        public List<GeocacheAttributeInfo> Attributes { get; set; }
        public List<LogCountInfo> LogCounts { get; set; }
        public List<GCComGeocacheImage> GeocacheImages { get; set; }
    }
}
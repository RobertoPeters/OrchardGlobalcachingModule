using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheSeriesFilter
    {
        public int BeginLength { get; set; }
        public int EndLength { get; set; }
        public int CountryID { get; set; }
    }

    public class GeocacheSeriesInfo
    {
        public int NumberOfCaches { get; set; }
        public string NameMatch { get; set; }
    }

    public class GeocacheSeriesModel
    {
        public GeocacheSeriesFilter Filter { get; set; }
        public List<GeocacheSeriesInfo> Geocaches { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
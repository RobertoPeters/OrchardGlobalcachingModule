using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheEventInfo
    {
        public int ID { get; set; }
    }

    public class EventCalendarModel
    {
        public List<GeocacheEventInfo> Events { get; set; }
    }
}
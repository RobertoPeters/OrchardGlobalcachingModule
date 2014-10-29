using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IEventCalendarService : IDependency
    {
        EventCalendarModel GetEvents();
    }

    public class EventCalendarService : IEventCalendarService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public EventCalendarModel GetEvents()
        {
            EventCalendarModel result = new EventCalendarModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result.Events = db.Fetch<GeocacheEventInfo>("select GCComGeocache.ID, GCComGeocache.Code, GCComGeocache.Name, GCComGeocache.UTCPlaceDate, GCComGeocache.Url, GCComGeocache.OwnerId, GCComGeocache.Latitude, GCComGeocache.Longitude, GCComGeocache.GeocacheTypeId from GCComGeocache where GCComGeocache.Archived=0 and GCComGeocache.GeocacheTypeId in (6, 13, 453) order by GCComGeocache.UTCPlaceDate desc");
            }
            return result;
        }
    }
}
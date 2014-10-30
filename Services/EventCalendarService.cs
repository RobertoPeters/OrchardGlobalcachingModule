using Gavaghan.Geodesy;
using Globalcaching.Core;
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

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public EventCalendarService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public EventCalendarModel GetEvents()
        {
            EventCalendarModel result = new EventCalendarModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result.Events = db.Fetch<GeocacheEventInfo>("select GCComGeocache.ID, GCComGeocache.Code, GCComGeocache.Name, GCComGeocache.UTCPlaceDate, GCComGeocache.Url, GCComGeocache.OwnerId, GCComGeocache.Latitude, GCComGeocache.Longitude, GCComGeocache.GeocacheTypeId, GCComUser.UserName, GCComUser.PublicGuid, GCComUser.AvatarUrl from GCComGeocache inner join GCComUser on GCComGeocache.OwnerId = GCComUser.ID  where GCComGeocache.Archived=0 and GCComGeocache.GeocacheTypeId in (6, 13, 453) order by GCComGeocache.UTCPlaceDate");
            }
            var settings = _gcEuUserSettingsService.GetSettings();
            foreach (var ev in result.Events)
            {
                if (settings != null && settings.HomelocationLat != null && settings.HomelocationLon != null && ev.Latitude != null && ev.Longitude != null)
                {
                    GeodeticMeasurement gm = Helper.CalculateDistance((double)settings.HomelocationLat, (double)settings.HomelocationLon, (double)ev.Latitude, (double)ev.Longitude);
                    ev.DirectionIcon = Helper.GetWindDirection(gm.Azimuth);
                    ev.DistanceFromHome = gm.EllipsoidalDistance / 1000.0;

                }
            }
            return result;
        }
    }
}
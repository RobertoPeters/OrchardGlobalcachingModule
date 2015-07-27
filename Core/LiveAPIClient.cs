using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Tucson.Geocaching.WCF.API.Geocaching1.Types;
using www.geocaching.com.Geocaching1.Live.data;

namespace Globalcaching.Core
{
    public class LiveAPIClient
    {
        public static LiveClient GetLiveClient()
        {
            return new LiveClient(ConfigurationManager.AppSettings["gccom_endpoint_live"]);
        }

        public static Tucson.Geocaching.WCF.API.Geocaching1.Types.UserProfile GetMemberProfile(string token)
        {
            Tucson.Geocaching.WCF.API.Geocaching1.Types.UserProfile result = null;
            LiveClient lc = GetLiveClient();
            try
            {
                var req = new GetYourUserProfileRequest();
                req.ProfileOptions = new YourUserProfileOptions();
                req.DeviceInfo = new Tucson.Geocaching.WCF.API.Geocaching1.Types.DeviceData();
                req.DeviceInfo.DeviceName = "globalcaching.eu";
                req.DeviceInfo.ApplicationSoftwareVersion = "V3.0.0.0";
                req.DeviceInfo.DeviceUniqueId = "internal";
                req.AccessToken = token;
                var resp = lc.GetYourUserProfile(req);

                if (resp.Status.StatusCode == 0)
                {
                    result = resp.Profile;
                }
            }
            catch
            {
            }
            lc.Close();
            return result;
        }


        public static GeocacheLog LogGeocache(string token, string GCCode, string logText, DateTime logDate, bool favorite)
        {
            GeocacheLog result = null;

            LiveClient lc = GetLiveClient();
            try
            {
                CreateFieldNoteAndPublishRequest cfnr = new CreateFieldNoteAndPublishRequest();
                cfnr.AccessToken = token;
                cfnr.CacheCode = GCCode;
                cfnr.Note = logText.Replace("\r\n", "\r").Replace("\r", "\r\n");
                cfnr.PromoteToLog = true;
                logDate = logDate.AddHours(12);
                cfnr.UTCDateLogged = logDate.ToUniversalTime();
                //cfnr.UTCDateLogged.Kind = DateTimeKind.Utc;
                cfnr.WptLogTypeId = 2;
                cfnr.FavoriteThisCache = favorite;
                CreateFieldNoteAndPublishResponse resp = lc.CreateFieldNoteAndPublish(cfnr);

                if (resp != null && resp.Status.StatusCode == 0)
                {
                    result = resp.Log;

                    //todo: add log to database!!
                }
            }
            catch
            {
                result = null;
            }
            lc.Close();
            return result;
        }

        public static bool LogTrackable(string token, string TrackableID, string logText, DateTime logDate)
        {
            bool result = false;

            LiveClient lc = GetLiveClient();
            try
            {
                /*
                string TrackingNumber = "";


                GetTrackableResponse tr = lc.GetTrackablesByTrackingNumber(GetWcfAccessToken(), TrackableID, 0);
                if (tr != null && tr.Status.StatusCode == 0)
                {
                    if (tr.Trackables != null && tr.Trackables.Length == 1)
                    {
                        TrackingNumber = tr.Trackables[0].Code;
                    }
                }
                */

                //System.Threading.Thread.Sleep(500);

                //test data: TB2CKRC-SB7N85
                //if (TrackingNumber.Length > 0)
                {
                    CreateTrackableLogRequest lr = new CreateTrackableLogRequest();
                    lr.AccessToken = token;
                    //lr.TravelBugCode = "TB2CKRC";
                    //lr.TrackingNumber = "SB7N85";
                    //lr.TravelBugCode = TrackingNumber;
                    lr.TrackingNumber = TrackableID;
                    lr.LogType = 48;
                    lr.Note = logText.Replace("\r\n", "\r").Replace("\r", "\r\n");
                    logDate = logDate.AddHours(12);
                    lr.UTCDateLogged = logDate.ToUniversalTime();
                    CreateTrackableLogResponse tlr = lc.CreateTrackableLog(lr);
                    if (tlr != null && tlr.Status.StatusCode == 0)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
            }
            lc.Close();
            return result;
        }

    }
}
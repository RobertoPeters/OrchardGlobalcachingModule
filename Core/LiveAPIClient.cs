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
                GetYourUserProfileRequest req = new GetYourUserProfileRequest();
                req.ProfileOptions = new Tucson.Geocaching.WCF.API.Geocaching1.Types.UserProfileOptions();
                req.DeviceInfo = new Tucson.Geocaching.WCF.API.Geocaching1.Types.DeviceData();
                req.DeviceInfo.DeviceName = "globalcaching.eu";
                req.DeviceInfo.ApplicationSoftwareVersion = "V3.0.0.0";
                req.DeviceInfo.DeviceUniqueId = "internal";
                req.AccessToken = token;
                GetUserProfileResponse resp = lc.GetYourUserProfile(req);

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

    }
}
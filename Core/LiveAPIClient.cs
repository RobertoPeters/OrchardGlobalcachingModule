using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
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

    }
}
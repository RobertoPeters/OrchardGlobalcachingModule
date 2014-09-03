using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface ILiveAPISettingsService : IDependency
    {
        GCComUser GetGeocachingComAccountInfo();
    }

    public class LiveAPISettingsService : ILiveAPISettingsService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public LiveAPISettingsService(IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        public GCComUser GetGeocachingComAccountInfo()
        {
            GCComUser result = null;
            GCEuUserSettings settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && !string.IsNullOrEmpty(settings.LiveAPIToken))
            {
                result = HttpContext.Session["GCComUser"] as GCComUser;
                if (result != null)
                {
                    if (result.ID == settings.GCComUserID)
                    {
                        //all ok
                    }
                    else
                    {
                        //wrong user
                        result = null;
                    }
                }
                if (result == null)
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        result = db.SingleOrDefault<GCComUser>("where ID=@0", settings.GCComUserID);
                    }
                    HttpContext.Session["GCComUser"] = result;
                }
            }
            else
            {
                return null;
            }
            return result;
        }
    }
}
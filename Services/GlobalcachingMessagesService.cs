using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGlobalcachingMessagesService : IDependency
    {
        GlobalcachingMessagesModel GetMessages();
    }

    public class GlobalcachingMessagesService : IGlobalcachingMessagesService
    {
        public static string dbTaskSchedulerConnString = ConfigurationManager.ConnectionStrings["SchedulerConnectionString"].ToString();

        public IOrchardServices Services { get; set; }
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService = null;

        public GlobalcachingMessagesService(IOrchardServices services,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            Services = services;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public GlobalcachingMessagesModel GetMessages()
        {
            GlobalcachingMessagesModel result = new GlobalcachingMessagesModel();
            result.ErrorMessages = new List<string>();
            result.InformationMessages = new List<string>();
            result.WarningMessages = new List<string>();
            result.UserSettings = _gcEuUserSettingsService.GetSettings();
            using (PetaPoco.Database db = new PetaPoco.Database(dbTaskSchedulerConnString, "System.Data.SqlClient"))
            {
                var scheduler = db.Fetch<SchedulerStatus>("").FirstOrDefault();
                if (scheduler.GCComWWWError)
                {
                    result.WarningMessages.Add("De website www.geocaching.com is niet bereikbaar.");
                }
                if (scheduler.LiveAPIError)
                {
                    result.WarningMessages.Add("De Live API service van www.geocaching.com is niet beschikbaar.");
                }
                if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
                {
                    var services = db.Fetch<ServiceInfo>("");
                    var dt = services.Max(x => x.LastRun);
                    if (dt != null)
                    {
                        if ((DateTime.Now - (DateTime)dt).TotalMinutes > 10)
                        {
                            result.ErrorMessages.Add("De task scheduler loopt niet.");
                        }
                    }
                    if (services.Where(x => x.ErrorInLastRun).FirstOrDefault() != null)
                    {
                        result.ErrorMessages.Add("Een taak in de task scheduler is fout gegaan.");
                    }
                }
            }
            return result;
        }

    }
}
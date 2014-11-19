using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface ITaskSchedulerService : IDependency
    {
        List<ScheduledWaypoint> GetScheduledGeocaches();
        void AddScheduledWaypoint(List<string> gcIds, bool fullRefresh);
        SchedulerInfoModel GetSchedulerInfoModel();
    }

    public class TaskSchedulerService : ITaskSchedulerService
    {
        public static string dbTaskSchedulerConnString = ConfigurationManager.ConnectionStrings["SchedulerConnectionString"].ToString();

        public TaskSchedulerService()
        { 
        }

        public SchedulerInfoModel GetSchedulerInfoModel()
        {
            SchedulerInfoModel result = new SchedulerInfoModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbTaskSchedulerConnString, "System.Data.SqlClient"))
            {
                result.Scheduler = db.Fetch<SchedulerStatus>("").FirstOrDefault();
                result.Services = db.Fetch<ServiceInfo>("");
                result.ServiceAccounts = db.Fetch<GcComAccounts>("");

                foreach (var sa in result.ServiceAccounts)
                {
                    sa.PM = false;
                    if (sa.Enabled && !string.IsNullOrEmpty(sa.Token))
                    {
                        try
                        {
                            var profile = LiveAPIClient.GetMemberProfile(sa.Token);
                            if (profile != null)
                            {
                                sa.PM = (profile.User.MemberType.MemberTypeId > 1);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }


        public List<ScheduledWaypoint> GetScheduledGeocaches()
        {
            List<ScheduledWaypoint> result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbTaskSchedulerConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<ScheduledWaypoint>("");
            }
            return result;
        }

        public void AddScheduledWaypoint(List<string> gcCodes, bool fullRefresh)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbTaskSchedulerConnString, "System.Data.SqlClient"))
            {
                var curList = db.Fetch<ScheduledWaypoint>("");
                foreach (string id in gcCodes)
                {
                    ScheduledWaypoint wp = curList.Where(x => string.Compare(x.Code, id, true)==0).FirstOrDefault();
                    if (wp != null)
                    {
                        if (fullRefresh && !wp.FullRefresh)
                        {
                            db.Execute("update ScheduledWaypoint set FullRefresh=1 where Code=@0", id);
                        }
                    }
                    else
                    {
                        wp = new ScheduledWaypoint();
                        wp.Code = id;
                        wp.FullRefresh = fullRefresh;
                        wp.DateAdded = DateTime.Now;
                        db.Insert(wp);
                    }
                }
            }
        }

    }
}
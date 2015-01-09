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
    public interface ITrackableGroupService: IDependency
    {
        TrackableGroupMaintModel GetGroupsOfCurrentUser(int userId, int? activeGroupId);
        TrackableGroupMaintModel AddGroup(int userId, GCEuTrackableGroup group);
        TrackableGroupMaintModel DeleteGroup(int userId, int id);
        TrackableGroupMaintModel AddTrackableToGroup(int userId, int id, string code);
        TrackableGroupMaintModel RemoveTrackableFromGroup(int userId, int id, string code);
    }

    public class TrackableGroupService : ITrackableGroupService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly ITaskSchedulerService _taskSchedulerService;

        public TrackableGroupService(ITaskSchedulerService taskSchedulerService)
        {
            _taskSchedulerService = taskSchedulerService;
        }

        public TrackableGroupMaintModel AddGroup(int userId, GCEuTrackableGroup group)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                group.UserID = userId;
                if (group.ID <= 0)
                {
                    group.CreatedAt = DateTime.Now;
                    group.ID = 0;
                    db.Insert(group);
                }
                else
                {
                    GCEuTrackableGroup m = db.FirstOrDefault<GCEuTrackableGroup>("where ID=@0 and UserID=@1", group.ID, userId);
                    if (m != null)
                    {
                        m.Name = group.Name;
                        m.Description = group.Description;
                        db.Save(m);
                    }
                }
            }
            return GetGroupsOfCurrentUser(userId, group.ID);
        }

        public TrackableGroupMaintModel DeleteGroup(int userId, int id)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCEuTrackableGroup m = db.FirstOrDefault<GCEuTrackableGroup>("where ID=@0 and UserID=@1", id, userId);
                if (m != null)
                {
                    db.Execute("delete from GCEuTrackable where GroupID=@0", id);
                    db.Execute("delete from GCEuTrackableGroup where ID=@0", id);
                }
            }
            return GetGroupsOfCurrentUser(userId, null);
        }

        public TrackableGroupMaintModel AddTrackableToGroup(int userId, int id, string code)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCEuTrackableGroup m = db.FirstOrDefault<GCEuTrackableGroup>("where ID=@0 and UserID=@1", id, userId);
                if (m != null)
                {
                    GCEuTrackable t = new GCEuTrackable();
                    t.Code = code;
                    t.GroupID = id;
                    db.Insert(t);
                    _taskSchedulerService.AddScheduledTrackable(code);
                }
            }
            return GetGroupsOfCurrentUser(userId, id);
        }

        public TrackableGroupMaintModel RemoveTrackableFromGroup(int userId, int id, string code)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCEuTrackableGroup m = db.FirstOrDefault<GCEuTrackableGroup>("where ID=@0 and UserID=@1", id, userId);
                if (m != null)
                {
                    db.Execute("delete from GCEuTrackable where GroupID=@0 and Code=@1", id, code);
                }
            }
            return GetGroupsOfCurrentUser(userId, id);
        }

        public TrackableGroupMaintModel GetGroupsOfCurrentUser(int userId, int? activeGroupId)
        {
            TrackableGroupMaintModel result = new TrackableGroupMaintModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.Groups = db.Fetch<GCEuTrackableGroup>("where UserID=@0 order by CreatedAt", userId);
                if (activeGroupId != null)
                {
                    result.ActiveGroup = (from a in result.Groups where a.ID == (int)activeGroupId select a).FirstOrDefault();
                }
                if (result.ActiveGroup == null && result.Groups.Count > 0)
                {
                    result.ActiveGroup = result.Groups[0];
                }
                if (result.ActiveGroup != null)
                {
                    result.TBCodes = db.Fetch<string>("select Code from GCEuTrackable where GroupID=@0 order by Code", result.ActiveGroup.ID);
                }
                else
                {
                    result.TBCodes = new List<string>();
                }
            }
            return result;
        }
    }
}
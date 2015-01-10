using Gavaghan.Geodesy;
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
    public interface ITrackableGroupService: IDependency
    {
        TrackableGroupMaintModel GetGroupsOfCurrentUser(int userId, int? activeGroupId);
        TrackableGroupMaintModel AddGroup(int userId, GCEuTrackableGroup group);
        TrackableGroupMaintModel DeleteGroup(int userId, int id);
        TrackableGroupMaintModel AddTrackableToGroup(int userId, int id, string code);
        TrackableGroupMaintModel RemoveTrackableFromGroup(int userId, int id, string code);
        TrackableGroupsModel GetTrackableGroups(int page, int pageSize);
        TrackableGroupMaintModel ScheduleTrackableOfGroup(int userId, int id, string code);
        TrackableGroupModel GetTrackableGroupData(int id);
    }

    public class TrackableGroupService : ITrackableGroupService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly ITaskSchedulerService _taskSchedulerService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public TrackableGroupService(ITaskSchedulerService taskSchedulerService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _taskSchedulerService = taskSchedulerService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public TrackableGroupModel GetTrackableGroupData(int id)
        {
            TrackableGroupModel result = new TrackableGroupModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.Trackables = db.Fetch<GCEuTrackable, GCComTrackable, TrackableInfo>((a, b) => { TrackableInfo c = new TrackableInfo(); c.GCEuTrackable = a; c.GCComTrackable = b; return c; }, "select GCEuTrackable.*, GCComTrackable.* from GCEuTrackable inner join GCComData.dbo.GCComTrackable on GCEuTrackable.Code = GCComTrackable.Code where GCEuTrackable.GroupID=@0 order by GCEuTrackable.Distance desc", id);
                result.Group = db.FirstOrDefault<GCEuTrackableGroup>("where ID=@0", id);
                if (result.Group != null)
                {
                    result.UserName = db.ExecuteScalar<string>("select top 1 Name from globalcaching.dbo.yaf_user where UserID=@0", result.Group.UserID);
                }
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null && settings.HomelocationLat != null && settings.HomelocationLon != null)
                {
                    foreach (var item in result.Trackables)
                    {
                        if (item.GCEuTrackable.Lat != null && item.GCEuTrackable.Lon != null)
                        {
                            GeodeticMeasurement gm = Helper.CalculateDistance((double)settings.HomelocationLat, (double)settings.HomelocationLon, (double)item.GCEuTrackable.Lat, (double)item.GCEuTrackable.Lon);
                            item.DirectionIcon = Helper.GetWindDirection(gm.Azimuth);
                            item.Distance = gm.EllipsoidalDistance / 1000.0;
                        }
                    }
                }
            }
            return result;
        }

        public TrackableGroupsModel GetTrackableGroups(int page, int pageSize)
        {
            TrackableGroupsModel result = new TrackableGroupsModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.PageCount = 1;
                result.CurrentPage = 1;
                var items = db.Page<TrackableGroupsInfo>(page, pageSize, "select GCEuTrackableGroup.ID, GCEuTrackableGroup.UserID, GCEuTrackableGroup.Name, GCEuTrackableGroup.CreatedAt, GCEuTrackableGroup.TrackableCount, yaf_user.Name as UserName from GCEuTrackableGroup inner join Globalcaching.dbo.yaf_user on GCEuTrackableGroup.UserID=yaf_user.UserID where GCEuTrackableGroup.TrackableCount>0");
                result.Groups = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
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
                if (m != null && db.ExecuteScalar<int>("select count(1) from GCEuTrackable where GroupID=@0 and Code=@1", id, code)==0)
                {
                    GCEuTrackable t = new GCEuTrackable();
                    t.Code = code;
                    t.GroupID = id;
                    db.Insert(t);
                    m.TrackableCount = db.ExecuteScalar<int>("select count(1) from GCEuTrackable where GroupID=@0", id);
                    _taskSchedulerService.AddScheduledTrackable(code);
                    db.Save(m);
                }
            }
            return GetGroupsOfCurrentUser(userId, id);
        }

        public TrackableGroupMaintModel ScheduleTrackableOfGroup(int userId, int id, string code)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCEuTrackableGroup m = db.FirstOrDefault<GCEuTrackableGroup>("where ID=@0 and UserID=@1", id, userId);
                if (m != null)
                {
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
                    m.TrackableCount = db.ExecuteScalar<int>("select count(1) from GCEuTrackable where GroupID=@0", id);
                    db.Save(m);
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
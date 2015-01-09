using Globalcaching.Models;
using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class TrackableGroupController: Controller
    {
        private readonly ITrackableGroupService _trackableGroupService;
        private readonly IGCEuUserSettingsService _userSettingsService;

        public TrackableGroupController(ITrackableGroupService trackableGroupService,
            IGCEuUserSettingsService userSettingsService)
        {
            _trackableGroupService = trackableGroupService;
            _userSettingsService = userSettingsService;
        }

        [HttpPost]
        public ActionResult DeleteGroup(string id)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                return Json(_trackableGroupService.DeleteGroup(settings.YafUserID, int.Parse(id)));
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveGroup(string id, string name, string description)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                GCEuTrackableGroup m = new GCEuTrackableGroup();
                m.ID = int.Parse(id);
                m.Description = description??"";
                m.Name = name ?? "";
                if (m.Name.Trim() == "")
                {
                    m.Name = "Naamloos";
                }
                return Json(_trackableGroupService.AddGroup(settings.YafUserID, m));
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddTrackable(string id, string code)
        {
            var settings = _userSettingsService.GetSettings();
            code = code.ToUpper().Trim();
            if (!string.IsNullOrEmpty(code) && code.StartsWith("TB"))
            {
                if (settings != null && settings.YafUserID > 1)
                {
                    return Json(_trackableGroupService.AddTrackableToGroup(settings.YafUserID, int.Parse(id), code));
                }
            }
            return null;
        }

        [HttpPost]
        public ActionResult DeleteTrackable(string id, string code)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                return Json(_trackableGroupService.RemoveTrackableFromGroup(settings.YafUserID, int.Parse(id), code));
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetGroupInfo(string id)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                return Json(_trackableGroupService.GetGroupsOfCurrentUser(settings.YafUserID, int.Parse(id)));
            }
            else
            {
                return null;
            }
        }

    }
}
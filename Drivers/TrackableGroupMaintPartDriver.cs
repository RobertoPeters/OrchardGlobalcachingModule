using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class TrackableGroupMaintPartDriver : ContentPartDriver<TrackableGroupMaintPart>
    {
        private readonly ITrackableGroupService _trackableGroupService;
        private readonly IGCEuUserSettingsService _userSettingsService;

        protected override string Prefix { get { return ""; } }

        public TrackableGroupMaintPartDriver(ITrackableGroupService trackableGroupService,
            IGCEuUserSettingsService userSettingsService)
        {
            _trackableGroupService = trackableGroupService;
            _userSettingsService = userSettingsService;
        }

        protected override DriverResult Display(TrackableGroupMaintPart part, string displayType, dynamic shapeHelper)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                return ContentShape("Parts_TrackableGroupMaint",
                        () => shapeHelper.DisplayTemplate(
                                TemplateName: "Parts.TrackableGroupMaint",
                                Model: _trackableGroupService.GetGroupsOfCurrentUser(settings.YafUserID, null),
                                Prefix: Prefix));
            }
            else
            {
                return null;
            }
        }
    }
}
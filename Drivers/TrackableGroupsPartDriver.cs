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
    public class TrackableGroupsPartDriver : ContentPartDriver<TrackableGroupsPart>
    {
        private readonly ITrackableGroupService _trackableGroupService;

        protected override string Prefix { get { return ""; } }

        public TrackableGroupsPartDriver(ITrackableGroupService trackableGroupService)
        {
            _trackableGroupService = trackableGroupService;
        }

        protected override DriverResult Display(TrackableGroupsPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_TrackableGroups",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.TrackableGroups",
                            Model: _trackableGroupService.GetTrackableGroups(1, 50),
                            Prefix: Prefix));
        }
    }
}
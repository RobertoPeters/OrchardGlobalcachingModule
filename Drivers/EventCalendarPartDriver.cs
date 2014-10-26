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
    public class EventCalendarPartDriver : ContentPartDriver<EventCalendarPart>
    {
        protected override string Prefix { get { return ""; } }
        private readonly IEventCalendarService _eventCalendarService;

        public EventCalendarPartDriver(IEventCalendarService eventCalendarService)
        {
            _eventCalendarService = eventCalendarService;
        }

        protected override DriverResult Display(EventCalendarPart part, string displayType, dynamic shapeHelper)
        {
            var m = _eventCalendarService.GetEvents();
            return ContentShape("Parts_EventCalendar",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.EventCalendar",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}
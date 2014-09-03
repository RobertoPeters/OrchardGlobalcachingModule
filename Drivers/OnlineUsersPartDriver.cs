using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class OnlineUsersPartDriver : ContentPartDriver<UsersOnlinePart>
    {
        private readonly IUsersOnlineService _usersOnlineService;

        protected override string Prefix { get { return ""; } }

        public OnlineUsersPartDriver(IUsersOnlineService usersOnlineService)
        {
            _usersOnlineService = usersOnlineService;
        }

        protected override DriverResult Display(UsersOnlinePart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_UsersOnline",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.UsersOnline",
                            Model: _usersOnlineService.GetOnLineUsers(),
                            Prefix: Prefix));
        }

    }
}
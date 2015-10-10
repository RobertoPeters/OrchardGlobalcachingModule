using System.Linq;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using Orchard;

namespace Globalcaching {
    public class AdminMenu : INavigationProvider {
        private readonly IAuthorizationService _authorizationService;

        public AdminMenu(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .Add(T("Globalcaching"), "1.0", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            menu.Add(T("Globalcaching"), "3",
                        item => item.Action("Index", "Admin", new { area = "Globalcaching" }).Permission(StandardPermissions.AccessAdminPanel));

        }
    }

    public class GlobalcachingAdminProvider : IMenuProvider
    {
        private readonly IAuthorizationService _authorizationService;
        private IOrchardServices Services { get; set; }

        public GlobalcachingAdminProvider(IAuthorizationService authorizationService,
            IOrchardServices services)
        {
            Services = services;
            _authorizationService = authorizationService;
        }

        public Localizer T { get; set; }

        public void GetMenu(Orchard.ContentManagement.IContent menu, NavigationBuilder builder)
        {
            builder.Add(T("Service aanroepen"), "1.0", item => item.Action("Index", "GlobalcachingServices", new { area = "Globalcaching" }).Permission(Permissions.GlobalAdmin));
            builder.Add(T("Lid instellingen"), "1.0", item => item.Action("ListMemberSettings", "EditUserSettings", new { area = "Globalcaching" }).Permission(Permissions.GlobalAdmin));
            builder.Add(T("GAPP autorizaties"), "1.0", item => item.Action("ListAuthorizations", "GAPPInfo", new { area = "Globalcaching" }).Permission(Permissions.GlobalAdmin));
            builder.Add(T("CCC deelnemers"), "1.0", item => item.Action("ListCCCMembers", "CheckCCC", new { area = "Globalcaching" }).Permission(Permissions.GlobalAdmin));
            builder.Add(T("Site Admin"), "1.0", item => item.Action("Index", "Admin", new { area = "Globalcaching" }).Permission(Permissions.GlobalAdmin));
            builder.Add(T("FTF Admin"), "1.0", item => item.Action("Index", "FTFStats", new { area = "Globalcaching" }).Permission(Permissions.FTFAdmin));
            builder.Add(T("Afstand Admin"), "1.0", item => item.Action("Index", "GeocacheDistance", new { area = "Globalcaching" }).Permission(Permissions.DistanceAdmin));
            builder.Add(T("Dashboard"), "1.0", item => item.Action("Index", "Admin", new { Area = "Dashboard" }).Permission(StandardPermissions.AccessAdminPanel));
            builder.Add(T("Contactform."), "1.0", item => item.Action("Index", "ContactForm", new { Area = "Globalcaching" }).Permission(Permissions.GlobalAdmin));
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin) || Services.Authorizer.Authorize(Permissions.FTFAdmin))
            {
                builder.Add(T("Activiteiten"), "1.0", item => item.Action("Index", "UsersOnline", new { Area = "Globalcaching" }));
            }            
        }

    }
}
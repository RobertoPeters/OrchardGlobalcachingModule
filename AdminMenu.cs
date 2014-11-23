using System.Linq;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Globalcaching {
    public class AdminMenu : INavigationProvider {
        private readonly IAuthorizationService _authorizationService;

        public AdminMenu(IAuthorizationService authorizationService) {
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

        public GlobalcachingAdminProvider(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public Localizer T { get; set; }

        public void GetMenu(Orchard.ContentManagement.IContent menu, NavigationBuilder builder)
        {
            builder.Add(T("Site Admin"), "1.0", item => item.Action("Index", "Admin", new { area = "Globalcaching" }).Permission(StandardPermissions.AccessAdminPanel));
            builder.Add(T("FTF Admin"), "1.0", item => item.Action("Index", "FTFStats", new { area = "Globalcaching" }).Permission(Permissions.FTFAdmin));
            builder.Add(T("Afstand Admin"), "1.0", item => item.Action("Index", "GeocacheDistance", new { area = "Globalcaching" }).Permission(Permissions.DistanceAdmin));
        }

    }
}
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
            menu.Add(T("Manage Blogs"), "3",
                        item => item.Action("Index", "Admin", new { area = "Globalcaching" }).Permission(StandardPermissions.AccessAdminPanel));

        }
    }
}
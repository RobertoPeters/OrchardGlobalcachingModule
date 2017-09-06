using Globalcaching.Services;
using Orchard;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class ShopController : Controller
    {
        public IOrchardServices Services { get; set; }
        private readonly IShopService _shopService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IGCEuUserSettingsService _userSettingsService;

        public ShopController(IOrchardServices services,
            IWorkContextAccessor workContextAccessor,
            IShopService shopService,
            IGCEuUserSettingsService userSettingsService)
        {
            Services = services;
            _workContextAccessor = workContextAccessor;
            _shopService = shopService;
            _userSettingsService = userSettingsService;
        }

        [Themed]
        public ActionResult ShopInfo()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                var m = _shopService.GetShopAdminModel();
                return View("Home", m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpGet]
        public ActionResult Authorize()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                var url = Url.Action("GetAccessToken", "Shop", new { area = "Globalcaching" });
                url = string.Format("{0}://{1}{2}", _workContextAccessor.GetContext().HttpContext.Request.Url.Scheme, _workContextAccessor.GetContext().HttpContext.Request.Headers["Host"], url);
                return Redirect(_shopService.Authorize(url));
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpGet]
        public ActionResult RefreshAccessToken()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                _shopService.RefreshAccessToken();
                return RedirectToAction("ShopInfo");
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpGet]
        public ActionResult GetAccessToken(string code, string state)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                var url = Url.Action("GetAccessToken", "Shop", new { area = "Globalcaching" });
                url = string.Format("{0}://{1}{2}", _workContextAccessor.GetContext().HttpContext.Request.Url.Scheme, _workContextAccessor.GetContext().HttpContext.Request.Headers["Host"], url);
                _shopService.GetAccessToken(code, state, url);
            }
            return RedirectToAction("ShopInfo");
        }

        [HttpPost]
        public ActionResult SetMasterCategoryId(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                _shopService.SetMasterCategoryId(id);
            }
            return RedirectToAction("ShopInfo");
        }

        [HttpPost]
        public ActionResult GetUserProduct(int id)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                return Json(_shopService.GetUserProduct(settings.YafUserID, id));
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteUserProduct(int id)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                return Json(_shopService.DeleteUserProduct(settings.YafUserID, id));
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddUserProduct(string name, int categoryId, string shortDescription, string fullDescription, string price)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                var m = _shopService.AddUserProduct(settings.YafUserID, name, categoryId, shortDescription, fullDescription, double.Parse(price.Replace(',','.'), CultureInfo.InvariantCulture));
                if (m != null)
                {
                    return Json(m);
                }
                else
                {
                    return Content("");
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveUserProduct(int id, string name, int categoryId, string shortDescription, string fullDescription, string price)
        {
            var settings = _userSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                var m = _shopService.SaveUserProduct(settings.YafUserID, id, name, categoryId, shortDescription, fullDescription, double.Parse(price.Replace(',', '.'), CultureInfo.InvariantCulture));
                if (m != null)
                {
                    return Json(m);
                }
                else
                {
                    return Content("");
                }
            }
            else
            {
                return null;
            }
        }

    }
}

using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class GCApprovedController : Controller
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public IOrchardServices Services { get; set; }
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GCApprovedController(IOrchardServices services,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            Services = services;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        [Themed]
        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin)
                || Services.Authorizer.Authorize(Permissions.ApprovedEditor)
                || Services.Authorizer.Authorize(Permissions.ApprovedViewer))
            {
                return View("Home", GetApprovedData(1,500,null,null));
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpPost]
        public ActionResult GetApproved(int page, int pageSize, string filter)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin)
                || Services.Authorizer.Authorize(Permissions.ApprovedEditor)
                || Services.Authorizer.Authorize(Permissions.ApprovedViewer))
            {
                var m = GetApprovedData(page, pageSize, null, filter);
                return Json(m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }


        public ActionResult GetApprovedRecord(int id, string filter)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin)
                || Services.Authorizer.Authorize(Permissions.ApprovedEditor)
                || Services.Authorizer.Authorize(Permissions.ApprovedViewer))
            {
                var m = GetApprovedData(1, 1, id, filter);
                return Json(m.EditItem);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteApprovedRecord(int id, string filter)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin)
                || Services.Authorizer.Authorize(Permissions.ApprovedEditor))
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    var record = db.FirstOrDefault<GCEuApprovedCode>("where ID=@0", id);
                    db.Delete(record);
                }
                return Json("OK");
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }


        [HttpPost]
        public ActionResult SaveApprovedRecord(int page, int pageSize, int id, string gccode, string comment, string filter)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin)
                || Services.Authorizer.Authorize(Permissions.ApprovedEditor))
            {
                GCApprovedCodeModel m = null;
                gccode = gccode.ToUpper().Trim();
                GCEuApprovedCode record = null;
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    if (id != 0)
                    {
                        record = db.FirstOrDefault<GCEuApprovedCode>("where ID=@0", id);
                        if (record.Code != gccode && db.FirstOrDefault<GCEuApprovedCode>("where Code=@0", gccode) != null)
                        {
                            record = null;
                        }
                    }
                    else
                    {
                        record = new GCEuApprovedCode();
                        record.ApprovedAt = DateTime.Now;
                        if (db.FirstOrDefault<GCEuApprovedCode>("where Code=@0", gccode) != null)
                        {
                            record = null;
                        }
                    }
                    if (record != null)
                    {
                        record.Code = gccode;
                        record.Comment = comment ?? "";
                        record.LastEditAt = DateTime.Now;
                        record.UserID = _gcEuUserSettingsService.GetSettings().YafUserID;
                        db.Save(record);

                        filter = record.Code;
                    }

                    m = GetApprovedData(page, pageSize, null, filter);
                    if (record != null)
                    {
                        m.EditItem = db.FirstOrDefault<GCApprovedCodeModelItem>("select GCEuApprovedCode.*, yaf_User.Name as UserName from GCEuApprovedCode inner join Globalcaching.dbo.yaf_User on GCEuApprovedCode.UserID = yaf_User.UserID where GCEuApprovedCode.ID=@0", record.ID);
                    }
                    else
                    {
                        m.EditItem = null;
                    }
                }
                return Json(m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public GCApprovedCodeModel GetApprovedData(int page, int pageSize, int? record, string filter)
        {
            var usrIdList = new List<int>();
            var result = new GCApprovedCodeModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.CanEdit = Services.Authorizer.Authorize(Permissions.ApprovedEditor);
            result.Filter = filter;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                PetaPoco.Page<GCApprovedCodeModelItem> items;
                if (string.IsNullOrEmpty(filter))
                {
                    items = db.Page<GCApprovedCodeModelItem>(page, pageSize, "select GCEuApprovedCode.*, yaf_User.Name as UserName from GCEuApprovedCode inner join Globalcaching.dbo.yaf_User on GCEuApprovedCode.UserID = yaf_User.UserID");
                }
                else
                {
                    items = db.Page<GCApprovedCodeModelItem>(page, pageSize, "select GCEuApprovedCode.*, yaf_User.Name as UserName from GCEuApprovedCode inner join Globalcaching.dbo.yaf_User on GCEuApprovedCode.UserID = yaf_User.UserID where GCEuApprovedCode.Code like @0", string.Format("%{0}%", filter));
                }
                result.Items = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.TotalCount = items.TotalItems;
                result.PageCount = items.TotalPages;

                if (record == null)
                {
                    result.EditItem = new GCApprovedCodeModelItem();
                    result.EditItem.ID = 0; //new
                }
                else
                {
                    result.EditItem = db.FirstOrDefault<GCApprovedCodeModelItem>("select GCEuApprovedCode.*, yaf_User.Name as UserName from GCEuApprovedCode inner join Globalcaching.dbo.yaf_User on GCEuApprovedCode.UserID = yaf_User.UserID where GCEuApprovedCode.ID=@0", record);
                }
            }
            return result;
        }

    }
}
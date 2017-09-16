using Globalcaching.Models;
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
    public class GlobalcachingDonationController : Controller
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public IOrchardServices Services { get; set; }

        public GlobalcachingDonationController(IOrchardServices services)
        {
            Services = services;
        }

        [Themed]
        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return View("Home", GetDonatorsData(1,500,null));
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpPost]
        public ActionResult GetDonators(int page, int pageSize)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                var m = GetDonatorsData(page, pageSize, null);
                m.EditItem = null;
                return View("Home", m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }


        public ActionResult GetDonatorRecord(int id)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                var m = GetDonatorsData(1, 1, id);
                return Json(m.EditItem);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveDonatorRecord(int page, int pageSize, int id, string nick, string partnernick, string name, string amount, string payedby, DateTime donationdate, DateTime expirationdate, string comment, bool remindersent, bool thankyousent)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                bool recordOK = true;
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    GCEuDonations record = new GCEuDonations();
                    record.ID = id;
                    record.Amount = Core.Helper.ConvertToDouble(amount);
                    record.Comment = comment ?? "";
                    record.DonatedAt = donationdate.Date.AddHours(12);
                    record.DonationMethod = payedby;
                    record.ExpirationDate = expirationdate.Date.AddHours(12);
                    var usrid = db.FirstOrDefault<int?>("select UserID from Globalcaching.dbo.yaf_User where Name=@0", nick);
                    if (usrid == null)
                    {
                        recordOK = false;
                    }
                    else
                    {
                        record.UserID = (int)usrid;
                    }
                    record.PartnerUserID = null;
                    if (!string.IsNullOrEmpty(partnernick))
                    {
                        usrid = db.FirstOrDefault<int?>("select UserID from Globalcaching.dbo.yaf_User where Name=@0", partnernick);
                        if (usrid == null)
                        {
                            recordOK = false;
                        }
                        else
                        {
                            record.PartnerUserID = usrid;
                        }
                    }
                    record.RealName = name ?? "";
                    record.ReminderSent = remindersent;
                    record.ThankYouSent = thankyousent;
                    if (recordOK)
                    {
                        db.Save(record);
                    }
                }

                var m = GetDonatorsData(page, pageSize, null);
                if (!recordOK)
                {
                    m.EditItem = null;
                }
                return Json(m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public GlobalcachingDonationModel GetDonatorsData(int page, int pageSize, int? record)
        {
            var usrIdList = new List<int>();
            GlobalcachingDonationModel result = new GlobalcachingDonationModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<GlobalcachingDonationItem>(page, pageSize, "select GCEuDonations.*, yaf_User.Name, PartnerName='', \"IsDonator\" = case when yaf_UserGroup.GroupID = 4 then 1 else 0 end from GCEuDonations left join Globalcaching.dbo.yaf_User on GCEuDonations.UserID = yaf_User.UserID left join Globalcaching.dbo.yaf_UserGroup on GCEuDonations.UserID = yaf_UserGroup.UserID and GroupID=4 order by ExpirationDate desc");
                result.Items = items.Items;
                foreach (var item in result.Items)
                {
                    if (item.PartnerUserID != null)
                    {
                        item.PartnerName = db.ExecuteScalar<string>("select Name from Globalcaching.dbo.yaf_User where UserID=@0", item.PartnerUserID);
                    }

                    if (!usrIdList.Contains(item.UserID))
                    {
                        usrIdList.Add(item.UserID);
                        if (item.PartnerUserID != null)
                        {
                            usrIdList.Add((int)item.PartnerUserID);
                        }
                        if (item.ExpirationDate > DateTime.Now)
                        {
                            item.IsInWrongState = !item.IsDonator;
                        }
                        else
                        {
                            item.IsInWrongState = item.IsDonator;
                        }
                    }
                }
                result.Items = result.Items.OrderByDescending(x => x.IsInWrongState).ThenByDescending(x => x.ExpirationDate).ToList();
                result.CurrentPage = items.CurrentPage;
                result.TotalCount = items.TotalItems;
                result.PageCount = items.TotalPages;

                if (record == null)
                {
                    result.EditItem = new GlobalcachingDonationItem();
                    result.EditItem.ID = 0; //new
                    result.EditItem.Amount = 10;
                    result.EditItem.Comment = "";
                    result.EditItem.DonatedAt = DateTime.Now;
                    result.EditItem.DonationMethod = "Bank/PayPal";
                    result.EditItem.ExpirationDate = result.EditItem.DonatedAt.AddYears(1);
                    result.EditItem.IsDonator = false;
                    result.EditItem.Name = "";
                    result.EditItem.PartnerName = "";
                    result.EditItem.PartnerUserID = null;
                    result.EditItem.RealName = "";
                    result.EditItem.ReminderSent = false;
                    result.EditItem.ThankYouSent = false;
                    result.EditItem.UserID = -1;
                }
                else
                {
                    result.EditItem = db.FirstOrDefault<GlobalcachingDonationItem>("select GCEuDonations.*, yaf_User.Name, PartnerName='', \"IsDonator\" = case when yaf_UserGroup.GroupID = 4 then 1 else 0 end from GCEuDonations left join Globalcaching.dbo.yaf_User on GCEuDonations.UserID = yaf_User.UserID left join Globalcaching.dbo.yaf_UserGroup on GCEuDonations.UserID = yaf_UserGroup.UserID and GroupID=4 where GCEuDonations.ID=@0", record);
                    if (result.EditItem.PartnerUserID != null)
                    {
                        result.EditItem.PartnerName = db.ExecuteScalar<string>("select Name from Globalcaching.dbo.yaf_User where UserID=@0", result.EditItem.PartnerUserID);
                    }
                }

                var sum = db.FirstOrDefault<double?>("select sum(amount) from GCEuDonations where YEAR(DonatedAt)=@0", DateTime.Now.Year-1);
                result.TotalLastYear = sum ?? 0;
                sum = db.FirstOrDefault<double?>("select sum(amount) from GCEuDonations where YEAR(DonatedAt)=@0", DateTime.Now.Year);
                result.TotalThisYear = sum ?? 0;
            }
            return result;
        }

    }
}
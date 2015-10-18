using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuDonations")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuDonations
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int? PartnerUserID { get; set; }
        public string RealName { get; set; }
        public DateTime DonatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string DonationMethod { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public bool ThankYouSent { get; set; }
        public bool ReminderSent { get; set; }
    }
}
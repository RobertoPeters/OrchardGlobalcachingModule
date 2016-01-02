using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class FinancialYearStatusModel
    {
        public int Year { get; set; }
        public double TotalCosts { get; set; }
        public double TotalIncome { get; set; }
        public DateTime MostRecentDonationDate { get; set; }
    }
}
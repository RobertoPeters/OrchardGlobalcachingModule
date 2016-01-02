using Gavaghan.Geodesy;
using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IFinancialYearStatusService : IDependency
    {
        FinancialYearStatusModel GetFinancialYearStatus(FinancialYearStatusPart part);
    }

    public class FinancialYearStatusService : IFinancialYearStatusService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public FinancialYearStatusService()
        {
        }

        public FinancialYearStatusModel GetFinancialYearStatus(FinancialYearStatusPart part)
        {
            FinancialYearStatusModel result = new FinancialYearStatusModel();
            result.Year = part.Year;
            result.TotalCosts = part.TotalCosts;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.TotalIncome = db.FirstOrDefault<double?>("select sum(amount) from GCEuDonations where YEAR(DonatedAt)=@0", result.Year) ?? 0;
                result.MostRecentDonationDate = db.FirstOrDefault<DateTime?>("select Max(DonatedAt) from GCEuDonations where YEAR(DonatedAt)=@0", result.Year) ?? DateTime.Now;
            }
            return result;
        }
    }
}
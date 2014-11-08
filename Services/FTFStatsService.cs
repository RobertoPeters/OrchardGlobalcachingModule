using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IFTFStatsService : IDependency
    {
        FTFStatsModel GetFTFRanking(string UserName, int Jaar, int RankingType, int page, int pageSize);
    }

    public class FTFStatsService : IFTFStatsService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public FTFStatsService()
        {
        }

        public FTFStatsModel GetFTFRanking(string UserName, int Jaar, int RankingType, int page, int pageSize)
        {
            FTFStatsModel result = new FTFStatsModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Jaar = Jaar;
            result.UserName = UserName ?? "";
            result.RankingType = RankingType;

            string whereUserClause;
            if (string.IsNullOrEmpty(UserName))
            {
                whereUserClause = "";
            }
            else
            {
                whereUserClause = string.Format(" and GCComData.dbo.GCComUser.UserName like '%{0}%'", UserName.Replace("'", "''").Replace("@", "@@"));
            }
            string orderClause;
            if (RankingType == 0)
            {
                orderClause = "PositionPoints";
            }
            else
            {
                orderClause = "Position";
            }
            string yearClause;
            if (Jaar > 2000)
            {
                yearClause = string.Format("Jaar = {0}", Jaar);
            }
            else
            {
                yearClause = "Jaar is null";
            }
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<FTFStatsInfo>(page, pageSize, string.Format("select GCEuFTFStats.*, GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, GCComUser.UserName from GCEuFTFStats inner join GCComData.dbo.GCComUser on GCEuFTFStats.UserID = GCComData.dbo.GCComUser.ID where {0} {1} order by {2}", yearClause, whereUserClause, orderClause));
                result.FTFInfo = items.Items.ToList();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }

    }
}
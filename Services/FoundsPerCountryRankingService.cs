using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IFoundsPerCountryRankingService : IDependency
    {
        FoundsPerCountryRankingModel GetRanking(int page, int pageSize, int year, int countryId, string nameFilter);
    }

    public class FoundsPerCountryRankingService : IFoundsPerCountryRankingService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public FoundsPerCountryRankingService()
        {
        }

        public FoundsPerCountryRankingModel GetRanking(int page, int pageSize, int year, int countryId, string nameFilter)
        {
            FoundsPerCountryRankingModel result = new FoundsPerCountryRankingModel();
            result.CountryID = countryId;
            result.NameFilter = nameFilter??"";
            result.RankYear = year;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.PageCount = 1;
                result.CurrentPage = 1;
                PetaPoco.Page<FoundsPerCountryRankingItem> items;
                if (string.IsNullOrEmpty(nameFilter))
                {
                    items = db.Page<FoundsPerCountryRankingItem>(page, pageSize, "select GCEuFoundsRanking.Ranking, GCEuFoundsRanking.Founds , GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.MemberTypeId, GCComUser.PublicGuid, GCComUser.UserName from GCEuFoundsRanking inner join GCComData.dbo.GCComUser on GCEuFoundsRanking.GCComUserID = GCComUser.ID where GCEuFoundsRanking.RankYear=@0 and GCEuFoundsRanking.CountryID=@1 order by GCEuFoundsRanking.Ranking", year, countryId);
                }
                else
                {
                    items = db.Page<FoundsPerCountryRankingItem>(page, pageSize, string.Format("select GCEuFoundsRanking.Ranking, GCEuFoundsRanking.Founds , GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.MemberTypeId, GCComUser.PublicGuid, GCComUser.UserName from GCEuFoundsRanking inner join GCComData.dbo.GCComUser on GCEuFoundsRanking.GCComUserID = GCComUser.ID where GCEuFoundsRanking.RankYear=@0 and GCEuFoundsRanking.CountryID=@1 and GCComData.UserName like '%{0}%' order by GCEuFoundsRanking.Ranking", nameFilter.Replace("'","''").Replace("@","@@")), year, countryId);
                }
                result.Items = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }

    }
}
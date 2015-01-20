using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface ICombiRankingService : IDependency
    {
        CombiRankingModel GetRanking(int page, int pageSize, int rankType, int year, string nameFilter);
    }

    public class CombiRankingService : ICombiRankingService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public CombiRankingService()
        {
        }

        public CombiRankingModel GetRanking(int page, int pageSize, int rankType, int year, string nameFilter)
        {
            CombiRankingModel result = new CombiRankingModel();
            result.NameFilter = nameFilter??"";
            result.RankYear = year;
            result.RankType = rankType;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.PageCount = 1;
                result.CurrentPage = 1;
                string syear;
                if (year == 0)
                {
                    syear = " is null ";
                }
                else
                {
                    syear = string.Format(" = {0} ", year);
                }
                List<CombiRankingItem> items;
                if (string.IsNullOrEmpty(nameFilter))
                {
                    if (rankType == 0)
                    {
                        items = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.Position + GCEuFoundsRanking.Ranking) as Points, GCEuFTFStats.Position as FTFPoints, GCEuFoundsRanking.Ranking as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=@0 where GCEuFTFStats.Jaar {0}) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID order by c.Ranking", syear), year);
                    }
                    else
                    {
                        items = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points desc) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.FTFCount*5 + GCEuFTFStats.STFCount*3 + GCEuFTFStats.TTFCount) as FTFPoints, (GCEuFTFStats.FTFCount*5 + GCEuFTFStats.STFCount*3 + GCEuFTFStats.TTFCount + GCEuFoundsRanking.Founds) as Points, GCEuFoundsRanking.Founds as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=@0 where GCEuFTFStats.Jaar {0}) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID order by c.Ranking", syear), year);
                    }
                }
                else
                {
                    if (rankType == 0)
                    {
                        items = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.Position + GCEuFoundsRanking.Ranking) as Points, GCEuFTFStats.Position as FTFPoints, GCEuFoundsRanking.Ranking as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=@0 where GCEuFTFStats.Jaar {0}) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID where GCComUser.UserName like '%{1}%' order by c.Ranking", syear, nameFilter.Replace("'", "''").Replace("@", "@@")), year);
                    }
                    else
                    {
                        items = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points desc) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.FTFCount*5 + GCEuFTFStats.STFCount*3 + GCEuFTFStats.TTFCount) as FTFPoints, (GCEuFTFStats.FTFCount*5 + GCEuFTFStats.STFCount*3 + GCEuFTFStats.TTFCount + GCEuFoundsRanking.Founds) as Points, GCEuFoundsRanking.Founds as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=0 where GCEuFTFStats.Jaar is null) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID where GCComUser.UserName like '%{1}%' order by c.Ranking", syear, nameFilter.Replace("'", "''").Replace("@", "@@")), year);
                    }
                }
                result.Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                result.CurrentPage = page;
                result.TotalCount = items.Count();
                result.PageCount = result.TotalCount / pageSize;
                if (result.TotalCount > 0 && (result.TotalCount % pageSize) != 0)
                {
                    result.PageCount++;
                }
            }
            return result;
        }

    }
}
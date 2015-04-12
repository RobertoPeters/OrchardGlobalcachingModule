using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface ICombiRankingService : IDependency
    {
        CombiRankingModel GetRanking(int page, int pageSize, int rankType, int year, string nameFilter);
        void CreateCombiBanner(HttpResponseBase response, string id, string year, string type);
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
                        items = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.PositionPoints + GCEuFoundsRanking.Ranking) as Points, GCEuFTFStats.PositionPoints as FTFPoints, GCEuFoundsRanking.Ranking as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=@0 where GCEuFTFStats.Jaar {0}) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID order by c.Ranking", syear), year);
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
                        items = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.PositionPoints + GCEuFoundsRanking.Ranking) as Points, GCEuFTFStats.PositionPoints as FTFPoints, GCEuFoundsRanking.Ranking as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=@0 where GCEuFTFStats.Jaar {0}) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID where GCComUser.UserName like '%{1}%' order by c.Ranking", syear, nameFilter.Replace("'", "''").Replace("@", "@@")), year);
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

        public class CombiBannerPoco
        {
            public long? GCComUserID { get; set; }
            public string UserName { get; set; }
        }
        public void CreateCombiBanner(HttpResponseBase response, string id, string year, string type)
        {
            string line1 = "";
            string line2 = "";

            if (type == null)
            {
                type = "1";
            }
            if (year == null)
            {
                year = "1";
            }

            if (id != null && id.Length > 0)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    var poco = db.Fetch<CombiBannerPoco>("select GCEuUserSettings.GCComUserID, GCComUser.UserName from GCEuUserSettings inner join GCComData.dbo.GCComUser on GCEuUserSettings.GCComUserID=GCComUser.ID where YafUserID=@0 and GCComUserID is not null", int.Parse(id)).FirstOrDefault();
                    if (poco != null)
                    {
                        line1 = string.Format("Combi van {0}", poco.UserName);
                        string syear;
                        if (year != "" && int.Parse(year) >= 2000)
                        {
                            syear = string.Format(" = {0}", int.Parse(year));
                            year = string.Format("{0} ", year);
                        }
                        else
                        {
                            year = "";
                            syear = " is null ";
                        }
                        CombiRankingItem combistat = null;
                        if (type == "2")
                        {
                            combistat = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points desc) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.FTFCount*5 + GCEuFTFStats.STFCount*3 + GCEuFTFStats.TTFCount) as FTFPoints, (GCEuFTFStats.FTFCount*5 + GCEuFTFStats.STFCount*3 + GCEuFTFStats.TTFCount + GCEuFoundsRanking.Founds) as Points, GCEuFoundsRanking.Founds as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=@0 where GCEuFTFStats.Jaar {0}) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID where UserID=@1", syear), year, poco.GCComUserID).FirstOrDefault();
                        }
                        else
                        {
                            combistat = db.Fetch<CombiRankingItem>(string.Format("select GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, c.Ranking, GCComUser.UserName, c.Points, c.FTFPoints, c.FoundsPoints from (select b.UserID, b.Points, b.FTFPoints, b.FoundsPoints, ROW_NUMBER() OVER (order by b.Points) as Ranking from (select GCEuFTFStats.UserID, (GCEuFTFStats.PositionPoints + GCEuFoundsRanking.Ranking) as Points, GCEuFTFStats.PositionPoints as FTFPoints, GCEuFoundsRanking.Ranking as FoundsPoints  from GCEuFTFStats inner join GCEuFoundsRanking on GCEuFTFStats.UserID = GCEuFoundsRanking.GCComUserID and GCEuFoundsRanking.CountryID=141 and GCEuFoundsRanking.RankYear=@0 where GCEuFTFStats.Jaar {0}) as b) as c inner join GCComData.dbo.GCComUser on c.UserID=GCComUser.ID where UserID=@1", syear), year, poco.GCComUserID).FirstOrDefault();
                        }
                        if (combistat != null)
                        {
                            if (type == "2")
                            {
                                line2 = string.Format("{0}Plaats: {1} ({2} punten)", year, combistat.Ranking, combistat.FTFPoints + combistat.FindCount);
                            }
                            else
                            {
                                line2 = string.Format("{0}Plaats: {1} ({2}/{3})", year, combistat.Ranking, combistat.FTFPoints, combistat.FoundsPoints);
                            }
                        }
                        else
                        {
                            if (type == "2")
                            {
                                line2 = string.Format("{0}Plaats: - (0 punten)", year);
                            }
                            else
                            {
                                line2 = string.Format("{0}Plaats: - (0/0/0)", year);
                            }
                        }
                    }
                    else
                    {
                        line1 = "Onbekend";
                    }
                }

            }
            CreateBanner(response, line1, line2);
        }

        private void CreateBanner(HttpResponseBase response, string line1, string line2)
        {
            using (Bitmap bitmap = new Bitmap(HttpContext.Current.Server.MapPath("/Modules/Globalcaching/Media/bannerv2.png"), true))
            using (MemoryStream memStream = new MemoryStream())
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                float l1startXPos = 5F;
                float l2startXPos = 25F;
                float l3startXPos = 25F;
                float defaultStartSize = 12f;

                float startSize = defaultStartSize;
                using (SolidBrush sb = new SolidBrush(Color.Black))
                {
                    Font font = new Font(FontFamily.GenericSansSerif, startSize, FontStyle.Bold);
                    SizeF sf = g.MeasureString(line1, font);
                    while (sf.Width > bitmap.Width - l1startXPos)
                    {
                        font.Dispose();
                        startSize -= 0.5F;
                        font = new Font(FontFamily.GenericSansSerif, startSize, FontStyle.Bold);
                        sf = g.MeasureString(line1, font);
                    }
                    g.DrawString(line1, font, sb, l1startXPos, 2F);
                    font.Dispose();


                    startSize = defaultStartSize;
                    font = new Font(FontFamily.GenericSansSerif, startSize, FontStyle.Regular);
                    sf = g.MeasureString(line2, font);
                    while (sf.Width > bitmap.Width - l2startXPos)
                    {
                        font.Dispose();
                        startSize -= 0.5F;
                        font = new Font(FontFamily.GenericSansSerif, startSize, FontStyle.Regular);
                        sf = g.MeasureString(line2, font);
                    }
                    g.DrawString(line2, font, sb, l2startXPos, 16F);
                    font.Dispose();

                    font = new Font(FontFamily.GenericSansSerif, defaultStartSize, FontStyle.Italic);
                    g.DrawString("Provided by Globalcaching.eu", font, sb, l3startXPos, 30F);
                    font.Dispose();


                    response.Clear();
                    response.ContentType = "image/png";
                    bitmap.Save(memStream, ImageFormat.Png);
                    memStream.WriteTo(response.OutputStream);

                }
            }
        }
    }
}
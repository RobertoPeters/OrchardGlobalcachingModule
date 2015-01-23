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
    public interface IFoundsPerCountryRankingService : IDependency
    {
        FoundsPerCountryRankingModel GetRanking(int page, int pageSize, int year, int countryId, string nameFilter);
        void CreateBanner(HttpResponseBase response, long id, int countryid, int year, int type);
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
                    items = db.Page<FoundsPerCountryRankingItem>(page, pageSize, string.Format("select GCEuFoundsRanking.Ranking, GCEuFoundsRanking.Founds , GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.MemberTypeId, GCComUser.PublicGuid, GCComUser.UserName from GCEuFoundsRanking inner join GCComData.dbo.GCComUser on GCEuFoundsRanking.GCComUserID = GCComUser.ID where GCEuFoundsRanking.RankYear=@0 and GCEuFoundsRanking.CountryID=@1 and GCComUser.UserName like '%{0}%' order by GCEuFoundsRanking.Ranking", nameFilter.Replace("'", "''").Replace("@", "@@")), year, countryId);
                }
                result.Items = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }

        public class BannerPoco
        {
            public int Ranking { get; set; }
            public int Founds { get; set; }
            public string UserName { get; set; }
        }
        public void CreateBanner(HttpResponseBase response, long id, int countryid, int year, int type)
        {
            string line1 = "";
            string line2 = "";

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                string scountry;
                var poco = db.Fetch<BannerPoco>("select GCEuFoundsRanking.Ranking, GCEuFoundsRanking.Founds, GCComUser.UserName from GCEuFoundsRanking inner join GCComData.dbo.GCComUser on GCEuFoundsRanking.GCComUserID = GCComUser.ID where GCEuFoundsRanking.RankYear=@0 and GCEuFoundsRanking.CountryID=@1 and GCEuFoundsRanking.GCComUserID=@2", year, countryid, id).FirstOrDefault();
                if (poco != null)
                {
                    switch (countryid)
                    {
                        case 141:
                            scountry = "NL";
                            break;
                        case 4:
                            scountry = "BE";
                            break;
                        case 8:
                            scountry = "LU";
                            break;
                        default:
                            scountry = "";
                            break;
                    }
                    line1 = string.Format("{0} stats van {1}", scountry, poco.UserName);
                    string syear;
                    if (year >= 2000)
                    {
                        syear = string.Format("{0} ", year);
                    }
                    else
                    {
                        syear = "";
                    }
                    if (type == 1)
                    {
                        line2 = string.Format("{0}Gevonden in {1}: {2}", syear, scountry, poco.Founds);
                    }
                    else
                    {
                        line2 = string.Format("{0}Plaats: {1} ({2})", syear, poco.Ranking, poco.Founds);
                    }
                }
                else
                {
                    line1 = "Onbekend";
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
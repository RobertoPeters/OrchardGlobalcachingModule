using Globalcaching.Models;
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
    public interface IFTFStatsService : IDependency
    {
        FTFStatsModel GetFTFRanking(string UserName, int Jaar, int RankingType, int page, int pageSize);
        FTFAdminModel GetUnassignedFTF(int page, int pageSize);
        bool SetFTFCompleted(long id);
        bool ClearFTFAssignment(long id);
        bool ClearSTFAssignment(long id);
        bool ClearTTFAssignment(long id);
        bool SetFTFAssignment(long geocacheId, long logId);
        bool SetSTFAssignment(long geocacheId, long logId);
        bool SetTTFAssignment(long geocacheId, long logId);
        void ResetFTFCounter(long id);
        void CreateFTFBanner(HttpResponseBase response, string id, string year, string type);
    }

    public class FTFStatsService : IFTFStatsService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public FTFStatsService()
        {
        }

        public void ResetFTFCounter(long id)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                if (id > 0)
                {
                    db.Execute("update GCEuGeocache set FTFFoundCount = FoundCount where ID=@0", id);
                }
                else
                {
                    db.Execute("update GCEuGeocache set FTFFoundCount = FoundCount");
                }
            }
        }

        public bool SetFTFAssignment(long geocacheId, long logId)
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCComGeocacheLog l = db.FirstOrDefault<GCComGeocacheLog>("select * from GCComData.dbo.GCComGeocacheLog where ID=@0", logId);
                if (l != null && l.FinderId!=null)
                {
                    result = db.Execute("update GCEuGeocache set FTFUserID = @0, FTFAtDate = @1 where ID=@2", l.FinderId, l.VisitDate, geocacheId) > 0;
                }
            }
            return result;
        }
        public bool SetSTFAssignment(long geocacheId, long logId)
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCComGeocacheLog l = db.FirstOrDefault<GCComGeocacheLog>("select * from GCComData.dbo.GCComGeocacheLog where ID=@0", logId);
                if (l != null && l.FinderId != null)
                {
                    result = db.Execute("update GCEuGeocache set STFUserID = @0, STFAtDate = @1 where ID=@2", l.FinderId, l.VisitDate, geocacheId) > 0;
                }
            }
            return result;
        }
        public bool SetTTFAssignment(long geocacheId, long logId)
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCComGeocacheLog l = db.FirstOrDefault<GCComGeocacheLog>("select * from GCComData.dbo.GCComGeocacheLog where ID=@0", logId);
                if (l != null && l.FinderId != null)
                {
                    result = db.Execute("update GCEuGeocache set TTFUserID = @0, TTFAtDate = @1 where ID=@2", l.FinderId, l.VisitDate, geocacheId) > 0;
                }
            }
            return result;
        }

        public bool ClearFTFAssignment(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set FTFUserID = NULL where ID=@0", id) > 0;
            }
            return result;
        }
        public bool ClearSTFAssignment(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set STFUserID = NULL where ID=@0", id) > 0;
            }
            return result;
        }
        public bool ClearTTFAssignment(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set TTFUserID = NULL where ID=@0", id) > 0;
            }
            return result;
        }

        public bool SetFTFCompleted(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set FTFCompleted=1 where ID=@0", id) > 0;
            }
            return result;
        }

        public FTFAdminModel GetUnassignedFTF(int page, int pageSize)
        {
            FTFAdminModel result = new FTFAdminModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Fetch<FTFAdminInfo>("select GCEuGeocache.ID, GCEuGeocache.FTFUserID, GCEuGeocache.STFUserID, GCEuGeocache.TTFUserID, GCEuGeocache.FoundCount, GCEuGeocache.FTFFoundCount, GCEuGeocache.PublishedAtDate, GCComGeocache.Code, GCComGeocacheType.GeocacheTypeName from GCEuGeocache inner join GCComData.dbo.GCComGeocache on GCComGeocache.ID = GCEuGeocache.ID inner join GCComData.dbo.GCComGeocacheType on GCComGeocacheType.ID = GCComGeocache.GeocacheTypeId where (FTFUserID is null or STFUserID is null or TTFUserID is null) and GCEuGeocache.FTFCompleted = 0 and GCComGeocache.CountryID = 141");
                foreach (var g in items)
                {
                    g.NewLogs = g.FoundCount - g.FTFFoundCount;
                }
                result.Geocaches = items.OrderByDescending(x => x.NewLogs).ThenBy(x => x.PublishedAtDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
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

        public class FTFBannerPoco
        {
            public long? GCComUserID { get; set; }
            public string UserName { get; set; }
        }
        public void CreateFTFBanner(HttpResponseBase response, string id, string year, string type)
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
                    var poco = db.Fetch<FTFBannerPoco>("select GCEuUserSettings.GCComUserID, GCComUser.UserName from GCEuUserSettings inner join GCComData.dbo.GCComUser on GCEuUserSettings.GCComUserID=GCComUser.ID where YafUserID=@0 and GCComUserID is not null", int.Parse(id)).FirstOrDefault();
                    if (poco != null)
                    {
                        line1 = string.Format("FTF van {0}", poco.UserName);
                        string yearClause;
                        if (year!="" && int.Parse(year) >= 2000)
                        {
                            yearClause = string.Format("Jaar = {0}", int.Parse(year));
                            year = string.Format("{0} ", year);
                        }
                        else
                        {
                            year = "";
                            yearClause = "Jaar is null";
                        }
                        var ftfstat = db.Fetch<FTFStatsInfo>(string.Format("select GCEuFTFStats.*, GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, GCComUser.UserName from GCEuFTFStats inner join GCComData.dbo.GCComUser on GCEuFTFStats.UserID = GCComData.dbo.GCComUser.ID where {0} and UserID=@0", yearClause), poco.GCComUserID).FirstOrDefault();
                        if (ftfstat != null)
                        {
                            if (type == "2")
                            {
                                line2 = string.Format("{0}Plaats: {1} ({2} punten)", year, ftfstat.PositionPoints, ftfstat.FTFCount * 5 + ftfstat.STFCount * 3 + ftfstat.TTFCount);
                            }
                            else
                            {
                                line2 = string.Format("{0}Plaats: {1} ({2}/{3}/{4})", year, ftfstat.Position, ftfstat.FTFCount, ftfstat.STFCount, ftfstat.TTFCount);
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
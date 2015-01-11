using Globalcaching.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class GlobalcachingServicesController: Controller
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public class CacheDistancePoco
        {
            public string Code { get; set; }
            public double? Distance { get; set; }
        }
        public class CacheFavoritesPoco
        {
            public string Code { get; set; }
            public int? FavoritePoints { get; set; }
        }
        public class GeocacheCodesPoco
        {
            public string Code { get; set; }
            public bool? Archived { get; set; }
            public bool? Available { get; set; }
        }
        public class GeoRSSPoco
        {
            public string Code { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public DateTime UTCPlaceDate { get; set; }
            public string GeocacheTypeName { get; set; }
        }

        public GlobalcachingServicesController()
        {

        }

        public ActionResult OldMainPage()
        {
            return Redirect("~/");
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult GeoRSS()
        {
            string country = Request.QueryString["country"] ?? "Netherlands";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<rss xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" version=\"2.0\">");
            sb.AppendLine("<channel>");
            sb.AppendLine("<title>Globalcaching.eu Generated RSS 2.0 Feed</title>");
            sb.AppendLine("<link>http://www.globalcaching.eu/Service/GeoRSS.aspx</link>");
            sb.AppendLine("<description>Geocache information</description>");
            sb.AppendLine(string.Format("<pubDate>{0}</pubDate>", DateTime.Now.ToString("R")));
            sb.AppendLine(string.Format("<lastBuildDate>{0}</lastBuildDate>", DateTime.Now.ToString("R")));
            sb.AppendLine("<generator>Globalcaching.eu</generator>");

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder.Append("SELECT Code, Latitude, Longitude, UTCPlaceDate, GeocacheTypeName FROM GCComGeocache inner join GCComGeocacheType on GCComGeocache.GeocacheTypeId=GCComGeocacheType.ID WHERE Archived=0 and Country=@0", country);
                List<GeoRSSPoco> codes = db.Fetch<GeoRSSPoco>(sql);
                foreach (var s in codes)
                {
                    double x, y;
                    if (LatLon.RDFromLatLong((double)s.Latitude, (double)s.Longitude, out x, out y))
                    {
                        sb.AppendLine("<item>");
                        sb.AppendLine(string.Format("<title>{0}</title>", s.Code));
                        sb.AppendLine(string.Format("<link>http://www.globalcaching.eu/GeocacheKaart?wp={0}</link>", s.Code));
                        sb.AppendLine(string.Format("<description>{0}</description>", s.GeocacheTypeName));
                        sb.AppendLine(string.Format("<pubDate>{0}</pubDate>", s.UTCPlaceDate.ToString("R")));
                        sb.AppendLine("<georss:where>");
                        sb.AppendLine("<gml:Point srsName=\"EPSG:28992\">");
                        sb.AppendLine(string.Format("<gml:pos>{0} {1}</gml:pos>", (int)x, (int)y));
                        sb.AppendLine("</gml:Point>");
                        sb.AppendLine("</georss:where> ");
                        sb.AppendLine("</item>");
                    }
                }
            }
            sb.AppendLine("</channel>");
            sb.AppendLine("</rss>");
            Response.Write(sb.ToString());
            Response.Flush();

            return null;
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult GeocacheCodes()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string country = Request.QueryString["country"];
                if (!string.IsNullOrEmpty(country))
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        var sql = PetaPoco.Sql.Builder.Append("SELECT Code, Archived, Available FROM GCComGeocache WHERE Country=@0", country);
                        List<GeocacheCodesPoco> codes = db.Fetch<GeocacheCodesPoco>(sql);
                        foreach (var s in codes)
                        {
                            sb.AppendLine(string.Format("{0},{1},{2}", s.Code, ((bool)s.Archived) ? "1" : "0", ((bool)s.Available) ? "1" : "0"));
                        }
                    }
                }
            }
            catch
            {
            }
            return Content(sb.ToString());
        }


        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult CacheFavorites()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string token = Request.QueryString["token"];
                if (!string.IsNullOrEmpty(token) && Core.LiveAPIClient.GetMemberProfile(token).User.MemberType.MemberTypeId>1)
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        var sql = PetaPoco.Sql.Builder.Append("SELECT Code, FavoritePoints FROM GCComGeocache WHERE Archived=0 AND FavoritePoints is not null");
                        List<CacheFavoritesPoco> codes = db.Fetch<CacheFavoritesPoco>(sql);
                        foreach (var s in codes)
                        {
                            sb.AppendLine(string.Format("{0},{1}", s.Code, s.FavoritePoints));
                        }
                    }
                }
            }
            catch
            {
            }
            return Content(sb.ToString());
        }


        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult CacheDistance()
        {
            Response.Clear();
            Response.ContentType = "application/zip";
            Response.AppendHeader("content-disposition", "attachment;filename=CacheDistance.zip");
            using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream oZipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(Response.OutputStream))
            {
                oZipStream.SetLevel(9); // maximum compression

                ZipEntry oZipEntry = new ZipEntry("globalcaching.xml");
                //byte[] obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes(content.ToString());
                //oZipEntry.Size = obuffer.Length;
                byte[] obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<wps>\r\n");
                oZipStream.PutNextEntry(oZipEntry);
                oZipStream.Write(obuffer, 0, obuffer.Length);
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                {
                    var sql = PetaPoco.Sql.Builder.Append("SELECT Code, Distance FROM GCComGeocache inner join GCEuData.dbo.GCEuGeocache on GCComGeocache.ID=GCEuGeocache.ID WHERE Archived=0 AND Distance is not null");
                    List<CacheDistancePoco> codes = db.Fetch<CacheDistancePoco>(sql);
                    foreach (var s in codes)
                    {
                        obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes(string.Format("<wp code=\"{0}\" dist=\"{1}\" />\r\n", s.Code, s.Distance.ToString().Replace(',', '.')));
                        oZipStream.Write(obuffer, 0, obuffer.Length);
                    }
                }
                obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes("</wps>");
                oZipStream.Write(obuffer, 0, obuffer.Length);

                oZipStream.Finish();
                oZipStream.Flush();
                oZipStream.Close();
            }
            Response.Flush();
            Response.End();
            return null;
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Archived()
        {
            string country = Request.QueryString["country"];
            if (!string.IsNullOrEmpty(country))
            {
                Response.Clear();
                Response.ContentType = "application/zip";
                Response.AppendHeader("content-disposition", "attachment;filename=Archived.zip");
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream oZipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(Response.OutputStream))
                {
                    oZipStream.SetLevel(9); // maximum compression

                    ZipEntry oZipEntry = new ZipEntry("globalcaching.xml");
                    //byte[] obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes(content.ToString());
                    //oZipEntry.Size = obuffer.Length;
                    byte[] obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<wps>\r\n");
                    oZipStream.PutNextEntry(oZipEntry);
                    oZipStream.Write(obuffer, 0, obuffer.Length);
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        var sql = PetaPoco.Sql.Builder.Append("SELECT Code FROM GCComGeocache WHERE Archived=1 AND Country=@0", country);
                        List<string> codes = db.Fetch<string>(sql);
                        foreach (var s in codes)
                        {
                            obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes(string.Format("<wp>{0}</wp>\r\n", s));
                            oZipStream.Write(obuffer, 0, obuffer.Length);
                        }
                    }
                    obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes("</wps>");
                    oZipStream.Write(obuffer, 0, obuffer.Length);

                    oZipStream.Finish();
                    oZipStream.Flush();
                    oZipStream.Close();
                }
                Response.Flush();
                Response.End();
            }
            return null;
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult PQSet(int max, int maxLastPQ, int countryID)
        {
            StringBuilder sb = new StringBuilder();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<PQSet>");
                sb.AppendLine("     <LatestVersion>1.0.0.0</LatestVersion>");

                var sql = PetaPoco.Sql.Builder.Append("SELECT UTCPlaceDate FROM GCComGeocache WHERE Archived=0 AND CountryID=@0 ORDER BY UTCPlaceDate ASC", countryID);
                List<DateTime> dates = db.Fetch<DateTime>(sql);

                int inSet = 0;
                string fromDate = "2001-01-01";
                string lastFinishedDate = "";
                string lastDate = "";
                int lastDayCount = 0;
                foreach(var dt in dates)
                {
                    string currentDate = dt.ToString("yyyy-MM-dd");

                    if (lastDate == currentDate)
                    {
                        lastDayCount++;
                    }
                    else
                    {
                        lastFinishedDate = lastDate;
                        lastDayCount = 1;
                        lastDate = currentDate;
                    }

                    inSet++;
                    if (inSet > max)
                    {
                        //go one day back
                        sb.AppendLine(string.Format("     <Set FromDate=\"{0}\" ToDate=\"{1}\" />", fromDate, lastFinishedDate));
                        inSet = lastDayCount;
                        fromDate = lastDate;
                    }
                }

                DateTime endTime;
                if (inSet > maxLastPQ)
                {
                    endTime = DateTime.Now;
                    sb.AppendLine(string.Format("     <Set FromDate=\"{0}\" ToDate=\"{1}-{2}-{3}\" />", fromDate, endTime.Year.ToString(), endTime.Month.ToString("00"), endTime.Day.ToString("00")));
                    endTime = endTime.AddDays(1);
                    fromDate = string.Format("{0}-{1}-{2}", endTime.Year.ToString(), endTime.Month.ToString("00"), endTime.Day.ToString("00"));
                }
                endTime = DateTime.Now.AddMonths(3);
                sb.AppendLine(string.Format("     <Set FromDate=\"{0}\" ToDate=\"{1}-{2}-{3}\" />", fromDate, endTime.Year.ToString(), endTime.Month.ToString("00"), endTime.Day.ToString("00")));

                sb.AppendLine("</PQSet>");
            }
            return Content(sb.ToString());
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult PQSetNL()
        {
            int max = 950;
            int maxLastPQ = 900;
            int.TryParse(Request["max"] ?? "950", out max);
            int.TryParse(Request["mlpq"] ?? "900", out maxLastPQ);
            return PQSet(max, maxLastPQ, 141);
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult PQSetBE()
        {
            int max = 950;
            int maxLastPQ = 900;
            int.TryParse(Request["max"] ?? "950", out max);
            int.TryParse(Request["mlpq"] ?? "900", out maxLastPQ);
            return PQSet(max, maxLastPQ, 4);
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult PQSetLU()
        {
            int max = 950;
            int maxLastPQ = 900;
            int.TryParse(Request["max"] ?? "950", out max);
            int.TryParse(Request["mlpq"] ?? "900", out maxLastPQ);
            return PQSet(max, maxLastPQ, 8);
        }

    }
}
﻿using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using ICSharpCode.SharpZipLib.Zip;
using Orchard;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class GlobalcachingServicesController: Controller
    {
        private static string ForumBaseUrl = ConfigurationManager.AppSettings["forumBaseUrl"].ToString();

        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public IOrchardServices Services { get; set; }
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IGCComSearchUserService _gcComSearchUserService;
        private readonly IWorkContextAccessor _workContextAccessor;

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
        public class CacheFavoritesWithFoundsPoco
        {
            public string Code { get; set; }
            public int? FavoritePoints { get; set; }
            public int FoundCount { get; set; }
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

        public GlobalcachingServicesController(IOrchardServices services,
            IGCEuUserSettingsService gcEuUserSettingsService,
            IGCComSearchUserService gcComSearchUserService,
            IWorkContextAccessor workContextAccessor)
        {
            Services = services;
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _gcComSearchUserService = gcComSearchUserService;
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        public ActionResult OldMainPage()
        {
            return Redirect("~/");
        }

        [Themed]
        public ActionResult Index()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return View("Home", GetServiceCallsPageModel(1, 500));
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpPost]
        public ActionResult GetServiceCallsPage(int page, int pageSize)
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                return Json(GetServiceCallsPageModel(page, pageSize));
            }
            else
            {
                return null;
            }
        }

        private GlobalcachingServiceCallModel GetServiceCallsPageModel(int page, int pageSize)
        {
            GlobalcachingServiceCallModel result = new GlobalcachingServiceCallModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<GlobalcachingServiceCallItem>(page, pageSize, "select GCEuServiceCall.*, GCComUser.UserName as GCComNick, yaf_User.Name as GlobalNick, GCComUser.MemberTypeId, \"IsDonator\" = case when yaf_UserGroup.GroupID = 4 then 1 else 0 end from GCEuServiceCall left join GCComData.dbo.GCComUser on GCEuServiceCall.GCComUserID = GCComUser.ID left join Globalcaching.dbo.yaf_User on GCEuServiceCall.UserID = yaf_User.UserID left join Globalcaching.dbo.yaf_UserGroup on GCEuServiceCall.UserID = yaf_UserGroup.UserID and GroupID=4 order by GCEuServiceCall.ID desc");
                result.Calls = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.TotalCount = items.TotalItems;
                result.PageCount = items.TotalPages;
            }
            return result;
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
                string token = Request.QueryString["token"];
                string usr = Request.QueryString["usr"];
                string pwd = Request.QueryString["pwd"];
                var settings = GetUserSettings("GeocacheCodes", token, usr, pwd);
                if (!string.IsNullOrEmpty(country) && settings != null && settings.IsDonator && !string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID != null)
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
        public ActionResult GeocacheCodesExFilter()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Land,Belgium,4");
            sb.AppendLine("Land,Netherlands,141");
            sb.AppendLine("Land,Luxembourg,8");
            var states = CachedData.Instance.StatesInfo.OrderBy(x => x.State).ToArray();
            foreach (var st in states)
            {
                sb.AppendLine(string.Format("Provincie,{0},{1}", st.State, st.StateID));
            }
            return Content(sb.ToString());
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult GeocacheCodesEx()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string token = Request.QueryString["token"];
                string usr = Request.QueryString["usr"];
                string pwd = Request.QueryString["pwd"];
                string m = Request.QueryString["m"];
                var settings = GetUserSettings("GeocacheCodesEx", token, usr, pwd);
                if (settings != null && settings.IsDonator && !string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID != null)
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        bool valid = false;
                        var sql = PetaPoco.Sql.Builder.Append("SELECT a.Code, a.Archived, a.Available FROM GCComGeocache a");

                        try
                        {
                            if (!string.IsNullOrEmpty(m))
                            {
                                if (db.ExecuteScalar<long>(string.Format("SELECT count(1) FROM GCEuMacroData.sys.tables WHERE name = 'macro_{0}_Resultaat'", settings.YafUserID)) > 0)
                                {
                                    sql = sql.Append(string.Format(" left join GCEuMacroData.dbo.macro_{0}_Resultaat b on a.ID = b.ID", settings.YafUserID));
                                    valid = true;
                                }
                                else
                                {
                                    m = null;
                                }
                            }
                        }
                        catch
                        {
                            m = null;
                        }

                        sql = sql.Append("WHERE 1=0");

                        try
                        {
                            string sc = Request.QueryString["c"];
                            if (!string.IsNullOrEmpty(sc))
                            {
                                var lst = (from a in sc.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select (int.Parse(a))).ToArray();
                                if (lst.Length > 0)
                                {
                                    sql = sql.Append(" OR a.CountryID in (@countries)", new { countries = lst });
                                    valid = true;
                                }
                            }
                        }
                        catch
                        {
                        }

                        try
                        {
                            string sc2 = Request.QueryString["s"];
                            if (!string.IsNullOrEmpty(sc2))
                            {
                                var lst2 = (from a in sc2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select (int.Parse(a))).ToArray();
                                if (lst2.Length > 0)
                                {
                                    sql = sql.Append("OR a.StateID in (@states)", new { states = lst2 });
                                    valid = true;
                                }
                            }
                        }
                        catch
                        {
                        }

                        try
                        {
                            string slat = Request.QueryString["lat"];
                            string slon = Request.QueryString["lon"];
                            string sr = Request.QueryString["r"];
                            if (!string.IsNullOrEmpty(slat) && !string.IsNullOrEmpty(slon))
                            {
                                var lat = double.Parse(slat, CultureInfo.InvariantCulture);
                                var lon = double.Parse(slon, CultureInfo.InvariantCulture);
                                var radius = double.Parse(sr, CultureInfo.InvariantCulture);
                                sql.Append("OR dbo.F_GREAT_CIRCLE_DISTANCE(a.Latitude, a.Longitude, @0, @1) < @2", lat, lon, radius);
                                valid = true;
                            }
                        }
                        catch
                        {
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(m))
                            {
                                sql = sql.Append("OR (b.ID is not null)");
                            }
                        }
                        catch
                        {
                        }


                        if (valid)
                        {
                            List<GeocacheCodesPoco> codes = db.Fetch<GeocacheCodesPoco>(sql);
                            foreach (var s in codes)
                            {
                                sb.AppendLine(string.Format("{0},{1},{2}", s.Code, ((bool)s.Archived) ? "1" : "0", ((bool)s.Available) ? "1" : "0"));
                            }
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
                string usr = Request.QueryString["usr"];
                string pwd = Request.QueryString["pwd"];
                var settings = GetUserSettings("CacheFavorites", token, usr, pwd);
                bool isPM = false;
                if (!string.IsNullOrEmpty(token))
                {
                    isPM = Core.LiveAPIClient.GetMemberProfile(token).User.MemberType.MemberTypeId > 1;
                }
                else if (!string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID != null)
                {
                    var gcu = _gcComSearchUserService.GetGeocachingComUser((long)settings.GCComUserID);
                    if (gcu != null)
                    {
                        isPM = gcu.MemberTypeId > 1;
                    }
                }
                if (settings != null && settings.IsDonator && isPM && !string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID != null)
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
        public ActionResult CacheFavoritesWithFoundCount()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string token = Request.QueryString["token"];
                string usr = Request.QueryString["usr"];
                string pwd = Request.QueryString["pwd"];
                var settings = GetUserSettings("CacheFavoritesWithFoundCount", token, usr, pwd);
                bool isPM = false;
                if (!string.IsNullOrEmpty(token))
                {
                    isPM = Core.LiveAPIClient.GetMemberProfile(token).User.MemberType.MemberTypeId > 1;
                }
                else if (!string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID!=null)
                {
                    var gcu = _gcComSearchUserService.GetGeocachingComUser((long)settings.GCComUserID);
                    if (gcu != null)
                    {
                        isPM = gcu.MemberTypeId > 1;
                    }
                }
                if (settings != null && settings.IsDonator && isPM && !string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID != null)
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        var sql = PetaPoco.Sql.Builder.Append("SELECT GCComGeocache.Code, GCComGeocache.FavoritePoints, GCEuGeocache.FoundCount FROM GCComGeocache inner join GCEuData.dbo.GCEuGeocache on GCComGeocache.ID=GCEuGeocache.ID WHERE Archived=0 AND FavoritePoints is not null");
                        List<CacheFavoritesWithFoundsPoco> codes = db.Fetch<CacheFavoritesWithFoundsPoco>(sql);
                        foreach (var s in codes)
                        {
                            sb.AppendLine(string.Format("{0},{1},{2}", s.Code, s.FavoritePoints??0, s.FoundCount));
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
            string token = Request.QueryString["token"];
            string usr = Request.QueryString["usr"];
            string pwd = Request.QueryString["pwd"];
            var settings = GetUserSettings("CacheDistance", token, usr, pwd);
            if (settings != null && settings.IsDonator && !string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID != null)
            {
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
            }
            return null;
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Archived()
        {
            string country = Request.QueryString["country"];
            string token = Request.QueryString["token"];
            string usr = Request.QueryString["usr"];
            string pwd = Request.QueryString["pwd"];
            var settings = GetUserSettings("Archived", token, usr, pwd);
            if (settings != null && settings.IsDonator && !string.IsNullOrEmpty(settings.LiveAPIToken) && settings.GCComUserID != null)
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

        private GCEuUserSettings GetUserSettingsFromToken(string token)
        {
            GCEuUserSettings result = null;
            if (!string.IsNullOrEmpty(token))
            {
                var profile = LiveAPIClient.GetMemberProfile(token);
                if (profile != null)
                {
                    result = _gcEuUserSettingsService.GetSettings(profile.User.Id ?? -1);
                }
            }
            return result;
        }

        private GCEuUserSettings GetUserSettingsFromCredentials(string usr, string pwd)
        {
            GCEuUserSettings result = null;
            if (!string.IsNullOrEmpty(usr) && !string.IsNullOrEmpty(pwd))
            {
                try
                {
                    string url = ForumBaseUrl.Replace("/forum/", "");
                    System.Net.HttpWebRequest webRequest = System.Net.WebRequest.Create(string.Format("{0}/Layar/ccc.aspx?usr={1}&pwd={2}", url, HttpUtility.UrlEncode(usr), HttpUtility.UrlEncode(pwd))) as System.Net.HttpWebRequest;
                    webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.17 Safari/533.4";
                    using (System.IO.StreamReader responseReader = new System.IO.StreamReader(webRequest.GetResponse().GetResponseStream()))
                    {
                        // and read the response
                        string doc = responseReader.ReadToEnd();
                        if (doc == "OK")
                        {
                            result = _gcEuUserSettingsService.GetSettings(usr);
                        }
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        private GCEuUserSettings GetUserSettings(string ServiceName, string token, string usr, string pwd)
        {
            GCEuUserSettings result = null;
            if (!string.IsNullOrEmpty(token))
            {
                result = GetUserSettingsFromToken(token);
            }
            else
            {
                result = GetUserSettingsFromCredentials(usr, pwd);
            }
            var record = new GCEuServiceCall();
            record.CalledAt = DateTime.Now;
            record.IPAddress = HttpContext.Request.UserHostAddress ?? "";
            record.Credentials = (!string.IsNullOrEmpty(usr) && !string.IsNullOrEmpty(pwd));
            record.ServiceName = ServiceName;
            record.Token = !string.IsNullOrEmpty(token);
            if (result != null)
            {
                record.UserID = result.YafUserID;
                record.GCComUserID = result.GCComUserID;
            }
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                db.Insert(record);
            }

            return result;
        }
    }
}
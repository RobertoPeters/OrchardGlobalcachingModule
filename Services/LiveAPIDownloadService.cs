using Globalcaching.Core;
using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Tucson.Geocaching.WCF.API.Geocaching1.Types;
using www.geocaching.com.Geocaching1.Live.data;

namespace Globalcaching.Services
{
    public interface ILiveAPIDownloadService : IDependency
    {
        //geocaches
        LiveAPIDownloadStatus DownloadStatus { get; }
        bool SetMacroResultForDownload();
        bool SetQueryResultForDownload(PetaPoco.Sql sql);
        LiveAPIDownloadStatus StartDownload(bool isLite, string fileFormat);
        LiveAPIDownloadStatus StopDownload();
        string GetDownloadFilePath();
        string DownloadGPX(string Code);
        LiveAPIDownloadStatus UpdateLiveAPILimits();

        //logs
        GCEuDownloadLogsStatus DownloadLogStatus { get; }
        GCEuDownloadLogsStatus StartLogDownload(string names, bool inclYourArchived);
        GCComGeocacheLogLiveAPISearchResult GetLogs(int page, int pageSize, string txt = null, string ltids = null, DateTime? ldf = null, DateTime? ldt = null, DateTime? cdf = null, DateTime? cdt = null, int? ddiff = null, int? ddift = null, int? numpf = null, int? numpt = null, bool? arch = null, bool? osbnl = null, bool? enc = null);
    }

    public class LiveAPIDownloadService : ILiveAPIDownloadService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private static List<int> _activeDownloadsForUserID = new List<int>();

        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IMacroService _macroService;

        public class viewport
        {
            public double minLat { get; set; }
            public double maxLat { get; set; }
            public double minLon { get; set; }
            public double maxLon { get; set; }
        }

        public LiveAPIDownloadService(IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor,
            IMacroService macroService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
            _macroService = macroService;
        }

        public LiveAPIDownloadStatus DownloadStatus 
        {
            get
            {
                LiveAPIDownloadStatus result = null;
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    var settings = _gcEuUserSettingsService.GetSettings();
                    if (settings != null)
                    {
                        result = getOrCreateDownLoadStatus(db, settings.YafUserID);
                    }
                }
                return result;
            }
        }

        public GCEuDownloadLogsStatus DownloadLogStatus
        {
            get
            {
                GCEuDownloadLogsStatus result = null;
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    var settings = _gcEuUserSettingsService.GetSettings();
                    if (settings != null)
                    {
                        result = db.FirstOrDefault<GCEuDownloadLogsStatus>("select * from GCEuMacroData.dbo.GCEuDownloadLogsStatus where UserID=@0", settings.YafUserID);
                    }
                }
                return result;
            }
        }

        public GCEuDownloadLogsStatus StartLogDownload(string names, bool inclYourArchived)
        {
            GCEuDownloadLogsStatus result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null && settings.YafUserID>1)
                {
                    result = db.FirstOrDefault<GCEuDownloadLogsStatus>("select * from GCEuMacroData.dbo.GCEuDownloadLogsStatus where UserID=@0", settings.YafUserID);
                    if (result == null)
                    {
                        result = new GCEuDownloadLogsStatus();
                        result.Busy = null;
                        result.LastUpdateAt = DateTime.Now;
                        result.LogTableName = "";
                        result.Status = "Nieuw verzoek gedaan.";
                        result.TotalFindCount = null;
                        result.TotalLogsImported = 0;
                        result.UserID = settings.YafUserID;
                        result.UserNameBusy = "";
                        result.UserNamesCompleted = "";
                        result.IncludeYourArchived = inclYourArchived;
                        result.RequestedAt = DateTime.Now;
                        result.UserNames = names;
                        db.Insert("GCEuMacroData.dbo.GCEuDownloadLogsStatus", "UserID", false, result);
                    }
                    else
                    {
                        result.Busy = null;
                        result.Status = "Nieuw verzoek gedaan.";
                        result.IncludeYourArchived = inclYourArchived;
                        result.RequestedAt = DateTime.Now;
                        result.UserNames = names;
                        db.Update("GCEuMacroData.dbo.GCEuDownloadLogsStatus", "UserID", result);
                    }
                }
            }
            return result;
        }

        public GCComGeocacheLogLiveAPISearchResult GetLogs(int page, int pageSize, string txt = null, string ltids = null, DateTime? ldf = null, DateTime? ldt = null, DateTime? cdf = null, DateTime? cdt = null, int? ddiff = null, int? ddift = null, int? numpf = null, int? numpt = null, bool? arch = null, bool? osbnl = null, bool? enc = null)
        {
            var result = new GCComGeocacheLogLiveAPISearchResult();
            result.PageCount = 1;
            result.CurrentPage = 1;
            var status = DownloadLogStatus;
            if (status != null && status.Busy != null)
            {
                try
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        if (txt == null)
                        {
                            result.LogTypes = db.Fetch<GCComLogType>("where ID in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 22, 23, 24, 45, 46, 47, 74)");
                        }
                        var sql = PetaPoco.Sql.Builder.Append(string.Format("select {0}.*, GCComGeocache.GeocacheTypeId, GCComGeocache.Name, GCComGeocache.Url as GeocacheUrl, GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.MemberTypeId, GCComUser.PublicGuid, GCComUser.UserName, GCComLogType.WptLogTypeName", status.LogTableName))
                            .Append(string.Format("from GCEuMacroData.dbo.{0} with (nolock)", status.LogTableName))
                            .Append(string.Format("left join GCComGeocache with (nolock) on GCComGeocache.ID={0}.GeocacheID", status.LogTableName))
                            .Append(string.Format("inner join GCComUser with (nolock) ON {0}.FinderId=GCComUser.ID", status.LogTableName))
                            .Append(string.Format("inner join GCComLogType with (nolock) ON {0}.WptLogTypeId=GCComLogType.ID", status.LogTableName))
                            .Append("WHERE 1=1");
                        if (ldf != null && ldt != null && (ldf.Value.Date != new DateTime(2001, 1, 1) || ldt.Value.Date != DateTime.Now.Date.AddYears(1)))
                        {
                            sql = sql.Append("AND VisitDate BETWEEN @0 AND @1", ldf.Value.AddDays(-1), ldt.Value.AddDays(1));
                        }
                        if (cdf != null && ldt != null && (cdf.Value.Date != new DateTime(2001, 1, 1) || cdt.Value.Date != DateTime.Now.Date.AddYears(1)))
                        {
                            sql = sql.Append("AND UTCCreateDate BETWEEN @0 AND @1", cdf.Value.AddDays(-1), cdt.Value.AddDays(1));
                        }
                        if (ddiff != null && ddift != null && (ddiff.Value != 0 || ddift.Value != 999999))
                        {
                            sql = sql.Append("AND ABS(DATEDIFF(day,UTCCreateDate,VisitDate)) BETWEEN @0 AND @1", ddiff.Value-1, ddift.Value+1);
                        }
                        if (numpf != null && numpt != null && (numpf.Value != 0 || numpt.Value != 999999))
                        {
                            sql = sql.Append("AND NumberOfImages BETWEEN @0 AND @1", numpf.Value - 1, numpt.Value + 1);
                        }
                        if (arch != null && arch.Value)
                        {
                            sql = sql.Append("AND IsArchived = 1");
                        }
                        if (enc != null && enc.Value)
                        {
                            sql = sql.Append("AND LogIsEncoded = 1");
                        }
                        if (osbnl != null && osbnl.Value)
                        {
                            sql = sql.Append("AND GeocacheTypeId is null");
                        }
                        if (!string.IsNullOrWhiteSpace(txt))
                        {
                            sql = sql.Append(string.Format("AND LogText LIKE '%{0}%'", txt.Replace("'", "''").Replace("@", "@@")));
                        }
                        if (!string.IsNullOrWhiteSpace(ltids))
                        {
                            var parts = ltids.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            sql = sql.Append(string.Format("AND WptLogTypeId in ({0})", string.Join(",", (from a in parts select int.Parse(a)).ToArray())));
                        }
                        sql = sql.Append(string.Format("order by VisitDate desc, {0}.ID desc", status.LogTableName));
                        var items = db.Page<GCComGeocacheLogLiveAPI>(page, pageSize, sql);
                        result.Logs = items.Items.ToArray();
                        result.CurrentPage = items.CurrentPage;
                        result.PageCount = items.TotalPages;
                        result.TotalCount = items.TotalItems;

                        for (int i = 0; i < result.Logs.Length; i++)
                        {
                            //result.Logs[i].UTCCreateDate = result.Logs[i].UTCCreateDate.ToLocalTime();
                            result.Logs[i].LogText = HttpUtility.HtmlEncode(result.Logs[i].LogText ?? "").Replace("\r","<br />").Replace("\n","");
                            if (!string.IsNullOrWhiteSpace(txt))
                            {
                                int pos = result.Logs[i].LogText.IndexOf(txt, StringComparison.OrdinalIgnoreCase);
                                while (pos >= 0)
                                {
                                    string replaceStr = string.Format("<span style=\"background-color: #FFFF00\">{0}</span>", txt);
                                    result.Logs[i].LogText = string.Concat(result.Logs[i].LogText.Substring(0, pos), replaceStr, result.Logs[i].LogText.Substring(pos + txt.Length));
                                    pos = result.Logs[i].LogText.IndexOf(txt, pos + replaceStr.Length, StringComparison.OrdinalIgnoreCase);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    result.Logs = new GCComGeocacheLogLiveAPI[0];
                    result.TotalCount = 0;
                }
            }
            else
            {
                result.Logs = new GCComGeocacheLogLiveAPI[0];
                result.TotalCount = 0;
            }
            return result;            
        }


        private LiveAPIDownloadStatus getOrCreateDownLoadStatus(PetaPoco.Database db, int usrId)
        {
            LiveAPIDownloadStatus result = null;
            if (usrId < 1) return null;
            if (db.ExecuteScalar<int>("SELECT count(1) FROM GCEuMacroData.sys.tables WHERE name = 'LiveAPIDownloadStatus'") == 0)
            {
                //create table
                db.Execute(@"create table GCEuMacroData.dbo.LiveAPIDownloadStatus
(
UserID int not null,
TotalToDownload int not null,
Downloaded int not null,
Canceled bit not null,
StatusText ntext,
StartTime datetime,
FinishedTime datetime,
FileName nvarchar(255),
IsLite bit not null,
FileFormat nvarchar(255),
LiveAPICachesLeft int,
LiveAPIMaxCacheCount int,
LiveAPILastAccessTime datetime
)
");
            }
            result = db.FirstOrDefault<LiveAPIDownloadStatus>("select * from GCEuMacroData.dbo.LiveAPIDownloadStatus where UserID=@0", usrId);
            if (result == null)
            {
                result = new LiveAPIDownloadStatus();
                result.Canceled = false;
                result.Downloaded = 0;
                result.FinishedTime = null;
                result.StartTime = null;
                result.StatusText = "";
                result.TotalToDownload = -1;
                result.UserID = usrId;
                result.FileName = "";
                result.IsLite = false;
                result.FileFormat = "";
                result.LiveAPICachesLeft = null;
                result.LiveAPIMaxCacheCount = null;
                result.LiveAPILastAccessTime = null;

                db.Insert("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", false, result);
            }
            //if (result.TotalToDownload < 0)
            //{
                if (db.ExecuteScalar<int>(string.Format("SELECT count(1) FROM GCEuMacroData.sys.tables WHERE name = 'LiveAPIDownload_{0}_CachesToDo'", usrId)) == 0)
                {
                    //create table
                    db.Execute(string.Format("create table GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo (Code nvarchar(15) not null)", usrId));
                    result.TotalToDownload = 0;
                }
                else
                {
                    result.TotalToDownload = db.ExecuteScalar<int>(string.Format("SELECT count(1) FROM GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo", usrId));
                }
                db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", result);
            //}
            lock (_activeDownloadsForUserID)
            {
                result.IsDownloading = _activeDownloadsForUserID.Contains(usrId);
            }
            _macroService.DynamicTableCreated(db, string.Format("LiveAPIDownload_{0}_CachesToDo", usrId));

            return result;
        }

        public bool SetMacroResultForDownload()
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null && settings.YafUserID>0)
                {
                    result = SetQueryResultForDownload(PetaPoco.Sql.Builder.Append(string.Format("insert into GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo (Code) select Code from GCEuMacroData.dbo.macro_{0}_resultaat inner join GCComGeocache on GCEuMacroData.dbo.macro_{0}_resultaat.ID = GCComGeocache.ID", settings.YafUserID)));
                }
            }
            return result;
        }

        public bool SetQueryResultForDownload(PetaPoco.Sql sql)
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null && settings.YafUserID>1)
                {
                    var status = getOrCreateDownLoadStatus(db, settings.YafUserID);
                    if (status != null)
                    {
                        if (!status.IsDownloading)
                        {
                            if (!string.IsNullOrEmpty(status.FileName))
                            {
                                try
                                {
                                    if (System.IO.File.Exists(getFullFilePath(status.FileName)))
                                    {
                                        System.IO.File.Delete(getFullFilePath(status.FileName));
                                    }
                                }
                                catch
                                {
                                }
                            }
                            db.Execute(string.Format("truncate table GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo", settings.YafUserID));
                            db.Execute(sql);
                            status.TotalToDownload = db.ExecuteScalar<int>(string.Format("SELECT count(1) FROM GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo", settings.YafUserID));
                            status.StatusText = "";
                            status.FileName = "";
                            status.IsLite = false;
                            status.Downloaded = 0;
                            status.StartTime = null;
                            status.FinishedTime = null;
                            status.Canceled = false;
                            status.FileFormat = "";
                            db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", status);
                            result = true;
                        }
                        else
                        {
                            //download is still busy, need to cancel it first
                        }
                    }
                }
            }
            return result;
        }

        private static string _rootMapPath = null;
        private string getFullFilePath(string filename)
        {
            if (string.IsNullOrEmpty(_rootMapPath))
            {
                _rootMapPath = HttpContext.Current.Server.MapPath("/Modules/Globalcaching/GeneratedFiles");
            }
            return System.IO.Path.Combine(_rootMapPath, filename);
        }

        public string GetDownloadFilePath()
        {
            string result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null)
                {
                    var m = getOrCreateDownLoadStatus(db, settings.YafUserID);
                    if (m != null && !m.IsDownloading && !string.IsNullOrEmpty(m.FileName))
                    {
                        //check if ending with <gpx> if not, append it
                        result = getFullFilePath(m.FileName);
                        if (!hasGPXFileClosingTag(result))
                        {
                            System.IO.File.AppendAllText(result, "</gpx>", Encoding.UTF8);
                        }
                    }
                }
            }
            return result;
        }

        private bool hasGPXFileClosingTag(string filename)
        {
            bool result = false;
            if (System.IO.File.Exists(filename))
            {
                using (var reader = new System.IO.StreamReader(filename))
                {
                    if (reader.BaseStream.Length > 20)
                    {
                        reader.BaseStream.Seek(-16, System.IO.SeekOrigin.End);
                    }
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result = line == "</gpx>";
                        if (result) break;
                    }
                }
            }
            return result;
        }

        public LiveAPIDownloadStatus StopDownload()
        {
            LiveAPIDownloadStatus result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null)
                {
                    result = getOrCreateDownLoadStatus(db, settings.YafUserID);
                    if (result!=null && result.IsDownloading)
                    {
                        result.Canceled = true;
                        db.Execute("update GCEuMacroData.dbo.LiveAPIDownloadStatus set Canceled=1 where UserID=@0", settings.YafUserID);
                    }
                }
            }
            return result;
        }


        public LiveAPIDownloadStatus StartDownload(bool isLite, string fileFormat)
        {
            LiveAPIDownloadStatus result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null)
                {
                    result = getOrCreateDownLoadStatus(db, settings.YafUserID);
                    if (result == null) return null;
                    if (!string.IsNullOrEmpty(settings.LiveAPIToken))
                    {
                        if (result.TotalToDownload > 0)
                        {
                            if (result.IsDownloading)
                            {
                                result.StatusText = "Download is al gestart en nog bezig.";
                            }
                            else
                            {
                                //ok, we can start or resume
                                if (result.StartTime == null)
                                {
                                    result.StartTime = DateTime.Now;
                                }
                                result.Canceled = false;
                                result.IsDownloading = true;
                                result.IsLite = isLite;
                                result.FileFormat = fileFormat;
                                db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", result);

                                lock(_activeDownloadsForUserID)
                                {
                                    _activeDownloadsForUserID.Add(settings.YafUserID);
                                }

                                //before we out of context....
                                getFullFilePath("temp.txt");

                                System.Threading.Thread t = new System.Threading.Thread(() => ProcessDownloadThreadMethod(result, settings.LiveAPIToken, settings.YafUserID));
                                t.IsBackground = true;
                                t.Start();
                                return result;
                            }
                        }
                        else
                        {
                            result.StatusText = "Er staan geen caches in de lijst om te downloaden";
                        }
                    }
                    else
                    {
                        result.StatusText = "Je moet Globalcaching.eu autoriseren voor toegang tot geocaching.com";
                    }
                }
                db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", result);
            }
            return result;
        }

        private void ProcessDownloadThreadMethod(LiveAPIDownloadStatus status, string token, int YafUserID)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                try
                {
                    var AttributeTypes = db.Fetch<GCComAttributeType>("");

                    if (string.IsNullOrEmpty(status.FileName))
                    {
                        status.FileName = string.Format("{0}.gpx", Guid.NewGuid().ToString("N"));
                        status.StatusText = "Bezig met downloaden van de caches van geocaching.com...";
                        db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", status);
                    }
                    string gpxFile = getFullFilePath(status.FileName);

                    //check if partial file has been downloaded
                    if (hasGPXFileClosingTag(gpxFile))
                    {
                        //remove it
                        using (var fileStream = System.IO.File.OpenWrite(gpxFile))
                        {
                            fileStream.SetLength(fileStream.Length-6);
                        }
                    }

                    if (!System.IO.File.Exists(gpxFile))
                    {
                        var viewPort = db.Fetch<viewport>(string.Format("select Min(Latitude) as minLat, Max(Latitude) as maxLat, Min(Longitude) as minLon, Max(Longitude) as maxLon from GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo inner join GCComGeocache on GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo.Code = GCComGeocache.Code", YafUserID)).FirstOrDefault();
                        System.IO.File.AppendAllText(gpxFile, GPXStart(viewPort.minLat, viewPort.minLon, viewPort.maxLat, viewPort.maxLon, false), Encoding.UTF8);
                    }

                    using (var api = LiveAPIClient.GetLiveClient())
                    {
                        List<string> gcCodes = db.Fetch<string>(string.Format("select top 30 Code from GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo", YafUserID));
                        while (gcCodes.Count > 0 && !status.Canceled)
                        {
                            var req = new SearchForGeocachesRequest();
                            req.AccessToken = token;
                            req.CacheCode = new CacheCodeFilter();
                            req.CacheCode.CacheCodes = gcCodes.ToArray();
                            req.IsLite = status.IsLite;
                            req.MaxPerPage = gcCodes.Count;
                            req.GeocacheLogCount = 5;
                            var resp = api.SearchForGeocaches(req);
                            if (resp.Status.StatusCode == 0 && resp.Geocaches != null)
                            {
                                //add to GPX
                                foreach (var gc in resp.Geocaches)
                                {
                                    System.IO.File.AppendAllText(gpxFile, GPXForGeocache(gc, AttributeTypes), Encoding.UTF8);

                                    if (gc.AdditionalWaypoints != null)
                                    {
                                        foreach (var wp in gc.AdditionalWaypoints)
                                        {
                                            if (wp.Latitude != null && wp.Longitude != null)
                                            {
                                                System.IO.File.AppendAllText(gpxFile, GPXForWaypoint(wp), Encoding.UTF8);
                                            }
                                        }
                                    }
                                }

                                //remove from todo
                                string codesArray = string.Format("('{0}')", string.Join("', '", gcCodes));
                                db.Execute(string.Format("delete from GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo where Code in {1}", YafUserID, codesArray));

                                //update status
                                status.LiveAPILastAccessTime = DateTime.Now;
                                status.LiveAPICachesLeft = resp.CacheLimits.CachesLeft;
                                status.LiveAPIMaxCacheCount = resp.CacheLimits.MaxCacheCount;
                                status.Canceled = db.ExecuteScalar<bool>("select top 1 Canceled from GCEuMacroData.dbo.LiveAPIDownloadStatus where UserID=@0", YafUserID);
                                status.TotalToDownload -= resp.Geocaches.Length;
                                status.Downloaded += resp.Geocaches.Length;
                                status.StatusText = "Bezig met downloaden van de caches van geocaching.com..."; //todo: ETA
                                db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", status);

                                gcCodes = db.Fetch<string>(string.Format("select top 30 Code from GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo", YafUserID));
                                if (gcCodes.Count > 0 && !status.Canceled)
                                {
                                    System.Threading.Thread.Sleep(2100);
                                    status.Canceled = db.ExecuteScalar<bool>("select top 1 Canceled from GCEuMacroData.dbo.LiveAPIDownloadStatus where UserID=@0", YafUserID);
                                }
                            }
                            else
                            {
                                status.Canceled = true;
                                status.StatusText = resp.Status.StatusMessage;
                                db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", status);
                                break;
                            }
                        }

                        if (!status.Canceled)
                        {
                            System.IO.File.AppendAllText(gpxFile, "</gpx>", Encoding.UTF8);
                        }
                    }

                    if (!status.Canceled)
                    {
                        status.FinishedTime = DateTime.Now;
                        status.StatusText = "Alle geocaches met success gedownload van geocaching.com";
                        db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", status);
                    }
                }
                catch (Exception e)
                {
                    status.Canceled = true;
                    status.StatusText = e.Message;
                    db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", status);
                }
            }
            lock (_activeDownloadsForUserID)
            {
                _activeDownloadsForUserID.RemoveAll(x => x == YafUserID);
            }
        }

        public LiveAPIDownloadStatus UpdateLiveAPILimits()
        {
            LiveAPIDownloadStatus result = null;
            try
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    var settings = _gcEuUserSettingsService.GetSettings();
                    if (settings != null && !string.IsNullOrEmpty(settings.LiveAPIToken))
                    {
                        result = getOrCreateDownLoadStatus(db, settings.YafUserID);
                        if (result != null && !result.IsDownloading)
                        {
                            using (var api = LiveAPIClient.GetLiveClient())
                            {
                                var req = new SearchForGeocachesRequest();
                                req.AccessToken = settings.LiveAPIToken;
                                req.CacheCode = new CacheCodeFilter();
                                req.CacheCode.CacheCodes = new string[] { "GC" };
                                req.IsLite = result.IsLite;
                                req.MaxPerPage = 1;
                                req.GeocacheLogCount = 0;
                                var resp = api.SearchForGeocaches(req);
                                if (resp.Status.StatusCode == 0 && resp.Geocaches != null)
                                {
                                    result.LiveAPILastAccessTime = DateTime.Now;
                                    result.LiveAPICachesLeft = resp.CacheLimits.CachesLeft;
                                    result.LiveAPIMaxCacheCount = resp.CacheLimits.MaxCacheCount;
                                    db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", result);
                                }
                            }

                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public string DownloadGPX(string Code)
        {
            string result = null;
            try
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null && settings.GCComUserID > 0 && !string.IsNullOrEmpty(settings.LiveAPIToken))
                {
                    string fn = getFullFilePath(string.Format("{0}_{1}.gpx", settings.YafUserID, "GC"));
                    if (System.IO.File.Exists(fn))
                    {
                        System.IO.File.Delete(fn);
                    }

                    List<GCComAttributeType> AttributeTypes;
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                    {
                        AttributeTypes = db.Fetch<GCComAttributeType>("");
                    }

                    using (var api = LiveAPIClient.GetLiveClient())
                    {
                        var req = new SearchForGeocachesRequest();
                        req.AccessToken = settings.LiveAPIToken;
                        req.CacheCode = new CacheCodeFilter();
                        req.CacheCode.CacheCodes = new string[] { Code };
                        req.IsLite = !settings.IsPM;
                        req.MaxPerPage = 1;
                        req.GeocacheLogCount = 5;
                        var resp = api.SearchForGeocaches(req);
                        if (resp.Status.StatusCode == 0 && resp.Geocaches != null)
                        {
                            //add to GPX
                            foreach (var gc in resp.Geocaches)
                            {
                                System.IO.File.AppendAllText(fn, GPXStart((double)gc.Latitude, (double)gc.Longitude, (double)gc.Latitude, (double)gc.Longitude, true), Encoding.UTF8);
                                System.IO.File.AppendAllText(fn, GPXForGeocache(gc, AttributeTypes), Encoding.UTF8);

                                if (gc.AdditionalWaypoints != null)
                                {
                                    foreach (var wp in gc.AdditionalWaypoints)
                                    {
                                        if (wp.Latitude != null && wp.Longitude != null)
                                        {
                                            System.IO.File.AppendAllText(fn, GPXForWaypoint(wp), Encoding.UTF8);
                                        }
                                    }
                                }
                            }
                            System.IO.File.AppendAllText(fn, "</gpx>", Encoding.UTF8);
                            result = fn;
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        private string GPXStart(double minLat, double minLon, double maxLat, double maxLon, bool singleCache)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<gpx xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" version=\"1.0\" creator=\"Globalcaching Pocket Query\" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd http://www.groundspeak.com/cache/1/0/1 http://www.groundspeak.com/cache/1/0/1/cache.xsd\" xmlns=\"http://www.topografix.com/GPX/1/0\">");
            sb.AppendLine("  <name>Pocket Query</name>");
            if (singleCache)
            {
                sb.AppendLine("  <desc>This is an individual cache generated from Geocaching.com</desc>");
            }
            else
            {
                sb.AppendLine("  <desc>Geocache file generated by Globalcaching Website (HasChildren)</desc>");
            }
            sb.AppendLine("  <author>Globalcaching</author>");
            sb.AppendLine("  <email>globalcaching@gmail.com</email>");
            sb.AppendLine("  <url>http://www.globalcaching.eu</url>");
            sb.AppendLine("  <urlname>Geocaching - High Tech Treasure Hunting</urlname>");
            sb.AppendLine(string.Format("  <time>{0}Z</time>", DateTime.Now.ToUniversalTime().ToString("s")));
            sb.AppendLine("  <keywords>cache, geocache, globalcaching</keywords>");
            sb.AppendLine(string.Format("  <bounds minlat=\"{0}\" minlon=\"{1}\" maxlat=\"{2}\" maxlon=\"{3}\" />", minLat.ToString().Replace(',', '.'), minLon.ToString().Replace(',', '.'), maxLat.ToString().Replace(',', '.'), maxLon.ToString().Replace(',', '.')));
            return sb.ToString();
        }

        private string GPXForWaypoint(AdditionalWaypoint wp)
        {
            string result = "";
            XmlDocument doc = new XmlDocument();

            string shortTypeName = wp.Type;
            int p = wp.Type.IndexOf('|');
            if (p > 0)
            {
                shortTypeName = wp.Type.Substring(p + 1);
            }

            var wpt = doc.CreateElement("wpt");
            var attr = doc.CreateAttribute("lat");
            var txt = doc.CreateTextNode(wp.Latitude.ToString().Replace(',', '.'));
            attr.AppendChild(txt);
            wpt.Attributes.Append(attr);
            attr = doc.CreateAttribute("lon");
            txt = doc.CreateTextNode(wp.Longitude.ToString().Replace(',', '.'));
            attr.AppendChild(txt);
            wpt.Attributes.Append(attr);
            doc.AppendChild(wpt);

            var el = doc.CreateElement("time");
            txt = doc.CreateTextNode(string.Format("{0}Z", wp.UTCEnteredDate.ToString("s")));
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("name");
            txt = doc.CreateTextNode(wp.Code);
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("cmt");
            txt = doc.CreateTextNode(wp.Comment ?? "");
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("desc");
            if (!string.IsNullOrEmpty(wp.Description))
            {
                txt = doc.CreateTextNode(wp.Description);
            }
            else
            {
                txt = doc.CreateTextNode(shortTypeName);
            }
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("url");
            txt = doc.CreateTextNode(wp.Url);
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("urlname");
            if (string.IsNullOrEmpty(wp.UrlName))
            {
                txt = doc.CreateTextNode(wp.Description);
            }
            else
            {
                txt = doc.CreateTextNode(wp.UrlName);
            }
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("sym");
            txt = doc.CreateTextNode(shortTypeName);
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("type");
            txt = doc.CreateTextNode(string.Format("Waypoint|{0}", shortTypeName));
            el.AppendChild(txt);
            wpt.AppendChild(el);

            using (System.IO.TemporaryFile tmp = new System.IO.TemporaryFile(true))
            {
                doc.Save(tmp.Path);
                result = System.IO.File.ReadAllText(tmp.Path);
                result = string.Concat(result, "\r\n");
            }
            return validateXml(result);
        }

        private string GPXForGeocache(Geocache gc, List<GCComAttributeType> attributeTypes)
        {
            string result = "";
            XmlDocument doc = new XmlDocument();

            double lat = (double)gc.Latitude;
            double lon = (double)gc.Longitude;
            if (gc.UserWaypoints != null && gc.UserWaypoints.Length > 0)
            {
                lat = gc.UserWaypoints[0].Latitude;
                lon = gc.UserWaypoints[0].Longitude;
            }

            XmlElement wpt = doc.CreateElement("wpt");
            XmlAttribute attr = doc.CreateAttribute("lat");
            XmlText txt = doc.CreateTextNode(lat.ToString().Replace(',', '.'));
            attr.AppendChild(txt);
            wpt.Attributes.Append(attr);
            attr = doc.CreateAttribute("lon");
            txt = doc.CreateTextNode(lon.ToString().Replace(',', '.'));
            attr.AppendChild(txt);
            wpt.Attributes.Append(attr);
            doc.AppendChild(wpt);

            XmlElement el = doc.CreateElement("time");
            txt = doc.CreateTextNode(string.Format("{0}Z", gc.UTCPlaceDate.ToString("s")));
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("name");
            txt = doc.CreateTextNode(gc.Code);
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("desc");
            txt = doc.CreateTextNode(string.Format("{0} by {1}, {2} ({3}/{4})", gc.Name, gc.Owner.UserName, gc.CacheType.GeocacheTypeName, gc.Difficulty.ToString("0.#").Replace(',', '.'), gc.Terrain.ToString("0.#").Replace(',', '.')));
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("url");
            txt = doc.CreateTextNode(gc.Url);
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("urlname");
            txt = doc.CreateTextNode(gc.Name);
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("sym");
            if (gc.HasbeenFoundbyUser==true)
            {
                txt = doc.CreateTextNode("Geocache Found");
            }
            else
            {
                txt = doc.CreateTextNode("Geocache");
            }
            el.AppendChild(txt);
            wpt.AppendChild(el);

            el = doc.CreateElement("type");
            txt = doc.CreateTextNode(string.Format("Geocache|{0}", gc.CacheType.GeocacheTypeName));
            el.AppendChild(txt);
            wpt.AppendChild(el);

            XmlElement cache = doc.CreateElement("groundspeak_cache");
            wpt.AppendChild(cache);
            attr = doc.CreateAttribute("id");
            txt = doc.CreateTextNode(gc.ID.ToString());
            attr.AppendChild(txt);
            cache.Attributes.Append(attr);

            attr = doc.CreateAttribute("available");
            txt = doc.CreateTextNode(gc.Available.ToString().ToLower());
            attr.AppendChild(txt);
            cache.Attributes.Append(attr);

            attr = doc.CreateAttribute("archived");
            txt = doc.CreateTextNode(gc.Archived.ToString().ToLower());
            attr.AppendChild(txt);
            cache.Attributes.Append(attr);

            attr = doc.CreateAttribute("_xmlns_groundspeak");
            txt = doc.CreateTextNode("http://www.groundspeak.com/cache/1/0/1");
            attr.AppendChild(txt);
            cache.Attributes.Append(attr);

            el = doc.CreateElement("groundspeak_name");
            txt = doc.CreateTextNode(gc.Name);
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_placed_by");
            txt = doc.CreateTextNode(gc.PlacedBy);
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_owner");
            txt = doc.CreateTextNode(gc.Owner.UserName);
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_type");
            txt = doc.CreateTextNode(gc.CacheType.GeocacheTypeName);
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_container");
            txt = doc.CreateTextNode(gc.ContainerType.ContainerTypeName);
            el.AppendChild(txt);
            cache.AppendChild(el);

            XmlElement attrs = doc.CreateElement("groundspeak_attributes");
            cache.AppendChild(attrs);
            if (gc.Attributes != null)
            {
                foreach (var gcattr in gc.Attributes)
                {
                    int id = gcattr.AttributeTypeID;

                    el = doc.CreateElement("groundspeak_attribute");
                    var a = attributeTypes.Where(x => x.ID == gcattr.AttributeTypeID).FirstOrDefault();
                    if (a == null)
                    {
                        txt = doc.CreateTextNode(gcattr.AttributeTypeID.ToString());
                    }
                    else
                    {
                        txt = doc.CreateTextNode(a.Name);
                    }
                    el.AppendChild(txt);
                    attrs.AppendChild(el);
                    attr = doc.CreateAttribute("id");
                    txt = doc.CreateTextNode(id.ToString());
                    attr.AppendChild(txt);
                    el.Attributes.Append(attr);

                    attr = doc.CreateAttribute("inc");
                    if (gcattr.IsOn)
                    {
                        txt = doc.CreateTextNode("1");
                    }
                    else
                    {
                        txt = doc.CreateTextNode("0");
                    }
                    attr.AppendChild(txt);
                    el.Attributes.Append(attr);
                }
            }

            el = doc.CreateElement("groundspeak_difficulty");
            txt = doc.CreateTextNode(gc.Difficulty.ToString("0.#").Replace(',', '.'));
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_terrain");
            txt = doc.CreateTextNode(gc.Terrain.ToString("0.#").Replace(',', '.'));
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_country");
            txt = doc.CreateTextNode(gc.Country ?? "");
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_state");
            txt = doc.CreateTextNode(gc.State ?? "");
            el.AppendChild(txt);
            cache.AppendChild(el);

            el = doc.CreateElement("groundspeak_short_description");
            txt = doc.CreateTextNode(gc.ShortDescription ?? "");
            el.AppendChild(txt);
            cache.AppendChild(el);
            attr = doc.CreateAttribute("html");
            txt = doc.CreateTextNode(gc.ShortDescriptionIsHtml.ToString().ToLower());
            attr.AppendChild(txt);
            el.Attributes.Append(attr);

            el = doc.CreateElement("groundspeak_long_description");
            txt = doc.CreateTextNode(gc.LongDescription ?? "");
            el.AppendChild(txt);
            cache.AppendChild(el);
            attr = doc.CreateAttribute("html");
            txt = doc.CreateTextNode(gc.LongDescriptionIsHtml.ToString().ToLower());
            attr.AppendChild(txt);
            el.Attributes.Append(attr);

            el = doc.CreateElement("groundspeak_encoded_hints");
            txt = doc.CreateTextNode(gc.EncodedHints ?? "");
            el.AppendChild(txt);
            cache.AppendChild(el);

            XmlElement logsel = doc.CreateElement("groundspeak_logs");
            cache.AppendChild(logsel);
            if (gc.GeocacheLogs != null)
            {
                foreach (var l in gc.GeocacheLogs)
                {
                    XmlElement lel = doc.CreateElement("groundspeak_log");
                    logsel.AppendChild(lel);
                    attr = doc.CreateAttribute("id");
                    txt = doc.CreateTextNode(l.ID.ToString());
                    attr.AppendChild(txt);
                    lel.Attributes.Append(attr);

                    el = doc.CreateElement("groundspeak_date");
                    txt = doc.CreateTextNode(string.Format("{0}Z", l.VisitDate.ToString("s")));
                    el.AppendChild(txt);
                    lel.AppendChild(el);

                    el = doc.CreateElement("groundspeak_type");
                    txt = doc.CreateTextNode(l.LogType.WptLogTypeName ?? "");
                    el.AppendChild(txt);
                    lel.AppendChild(el);

                    el = doc.CreateElement("groundspeak_finder");
                    txt = doc.CreateTextNode(l.Finder.UserName);
                    el.AppendChild(txt);
                    lel.AppendChild(el);
                    attr = doc.CreateAttribute("id");
                    txt = doc.CreateTextNode(l.Finder.Id.ToString());
                    attr.AppendChild(txt);
                    el.Attributes.Append(attr);

                    el = doc.CreateElement("groundspeak_text");
                    txt = doc.CreateTextNode(l.LogText);
                    el.AppendChild(txt);
                    lel.AppendChild(el);
                    attr = doc.CreateAttribute("encoded");
                    txt = doc.CreateTextNode(l.LogIsEncoded.ToString().ToLower());
                    attr.AppendChild(txt);
                    el.Attributes.Append(attr);
                }
            }

            using (System.IO.TemporaryFile tmp = new System.IO.TemporaryFile(true))
            {
                doc.Save(tmp.Path);
                result = System.IO.File.ReadAllText(tmp.Path);
                result = result.Replace("<groundspeak_", "<groundspeak:");
                result = result.Replace("</groundspeak_", "</groundspeak:");
                result = result.Replace("_xmlns_groundspeak", "xmlns:groundspeak");
                result = string.Concat(result, "\r\n");
            }
            return validateXml(result);
        }

        public string validateXml(string doc)
        {
            string result = doc;
            result = result.Replace("&auml;", "&#228;");
            result = result.Replace("&uuml;", "&#252;");
            result = result.Replace("&szlig;", "&#223;");
            result = result.Replace("&Auml;", "&#196;");
            result = result.Replace("&Ouml;", "&#214;");
            result = result.Replace("&Uuml;", "&#220;");
            result = result.Replace("&nbsp;", "&#160;");
            result = result.Replace("&Agrave;", "&#192;");
            result = result.Replace("&Egrave;", "&#200;");
            result = result.Replace("&Eacute;", "&#201;");
            result = result.Replace("&Ecirc;", "&#202;");
            result = result.Replace("&egrave;", "&#232;");
            result = result.Replace("&eacute;", "&#233;");
            result = result.Replace("&ecirc;", "&#234;");
            result = result.Replace("&agrave;", "&#224;");
            result = result.Replace("&iuml;", "&#239;");
            result = result.Replace("&ugrave;", "&#249;");
            result = result.Replace("&ucirc;", "&#251;");
            result = result.Replace("&uuml;", "&#252;");
            result = result.Replace("&ccedil;", "&#231;");
            result = result.Replace("&AElig;", "&#198;");
            result = result.Replace("&aelig;", "&#330;");
            result = result.Replace("&OElig;", "&#338;");
            result = result.Replace("&oelig;", "&#339;");
            result = result.Replace("&euro;", "&#8364;");
            result = result.Replace("&laquo;", "&#171;");
            result = result.Replace("&raquo;", "&#187;");

            result = result.Replace("&#xE4;", "&#228;");
            result = result.Replace("&#xE5;", "&#229;");
            result = result.Replace("&#xF6;", "&#246;");
            result = result.Replace("&#xFC;", "&#252;");

            try
            {
                int pos;
                pos = result.IndexOf("&#x");
                while (pos >= 0)
                {
                    //for now, just forget the characters
                    //todo: convert to decimal
                    string subs = result.Substring(pos, result.IndexOf(';', pos) - pos + 1);
                    result = result.Replace(subs, "");
                    pos = result.IndexOf("&#x");
                }
            }
            catch
            {
            }

            return result;
        }

    }
}
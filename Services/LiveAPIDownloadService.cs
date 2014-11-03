using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface ILiveAPIDownloadService : IDependency
    {
        LiveAPIDownloadStatus DownloadStatus { get; }
        bool SetMacroResultForDownload();
        LiveAPIDownloadStatus StartDownload();
    }

    public class LiveAPIDownloadService : ILiveAPIDownloadService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public LiveAPIDownloadService(IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
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


        private LiveAPIDownloadStatus getOrCreateDownLoadStatus(PetaPoco.Database db, int usrId)
        {
            LiveAPIDownloadStatus result = null;
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
FinishedTime datetime
)
");
            }
            result = db.FirstOrDefault<LiveAPIDownloadStatus>("select count(1) from GCEuMacroData.dbo.LiveAPIDownloadStatus where UserID=@0", usrId);
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

                db.Insert("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", result);
            }
            if (result.TotalToDownload < 0)
            {
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
            }

            return result;
        }

        public bool SetMacroResultForDownload()
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null)
                {
                    var status = getOrCreateDownLoadStatus(db, settings.YafUserID);
                    if (status != null && status.FinishedTime!=null)
                    {
                        db.Execute(string.Format("truncate table GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo", settings.YafUserID));
                        db.Execute(string.Format("insert into GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo (Code) from (select GCComGeocache.Code from GCEuMacroData.dbo.macro_{0}_resultaat inner join GCComGeocaches on GCEuMacroData.dbo.macro_{0}_resultaat.ID = GCComGeocaches.ID) as t", settings.YafUserID));
                        status.TotalToDownload = db.ExecuteScalar<int>(string.Format("SELECT count(1) FROM GCEuMacroData.dbo.LiveAPIDownload_{0}_CachesToDo", settings.YafUserID));
                        status.StatusText = "";
                        db.Update("GCEuMacroData.dbo.LiveAPIDownloadStatus", "UserID", result);
                    }
                }
            }
            return result;
        }

        public LiveAPIDownloadStatus StartDownload()
        {
            LiveAPIDownloadStatus result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings != null)
                {
                    result = getOrCreateDownLoadStatus(db, settings.YafUserID);

                    //todo: check live api key, check to download>0, check not already busy downloading
                }
            }
            return result;
        }

    }
}
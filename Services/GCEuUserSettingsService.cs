using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGCEuUserSettingsService : IDependency
    {
        GCEuUserSettings GetSettings();
        GCEuUserSettings GetSettings(string userName);
        void UpdateSettings(GCEuUserSettings settings);
        string GetEMail(int yafUserID);
        List<ListMemberSettingsModel> GetAllMemberSettings();
    }

    public class GCEuUserSettingsService : IGCEuUserSettingsService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbYafForumConnString = ConfigurationManager.ConnectionStrings["yafnet"].ToString();
        public static string dbTaskSchedulerConnString = ConfigurationManager.ConnectionStrings["SchedulerConnectionString"].ToString();

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IOrchardServices _orchardServices;

        public GCEuUserSettingsService(IWorkContextAccessor workContextAccessor,
            IOrchardServices orchardServices)
        {
            _workContextAccessor = workContextAccessor;
            _orchardServices = orchardServices;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        public List<ListMemberSettingsModel> GetAllMemberSettings()
        {
            List<ListMemberSettingsModel> result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<ListMemberSettingsModel>("select GCEuUserSettings.*, yaf_user.Name from GCEuUserSettings inner join Globalcaching.dbo.yaf_user on GCEuUserSettings.YafUserID = yaf_user.UserID order by Name");
            }
            return result;
        }

        public string GetEMail(int yafUserID)
        {
            string result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbYafForumConnString, "System.Data.SqlClient"))
            {
                var idl = db.Fetch<string>("select EMail from Yaf_User where UserID=@0", yafUserID);
                if (idl != null && idl.Count > 0)
                {
                    result = idl[0];
                }
            }
            return result;
        }

        public GCEuUserSettings GetSettings(string userName)
        {
            GCEuUserSettings result = null;
            int? YafUserID = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbYafForumConnString, "System.Data.SqlClient"))
            {
                var idl = db.Fetch<int?>("select UserID from Yaf_User where Name=@0", userName);
                if (idl != null && idl.Count > 0)
                {
                    YafUserID = (int)idl[0];
                }
            }
            if (YafUserID != null)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    result = db.FirstOrDefault<GCEuUserSettings>("where YafUserID = @0", YafUserID);
                }
            }
            return result;
        }

        public GCEuUserSettings GetSettings()
        {
            GCEuUserSettings result = null;
            if (_orchardServices.WorkContext.CurrentUser != null)
            {
                result = HttpContext.Session["GCEuUserSettings"] as GCEuUserSettings;
                if (result != null)
                {
                    //just to be sure, check it!
                    if (HttpContext.Session["GCEuUserSettingsForUser"] as string == _orchardServices.WorkContext.CurrentUser.UserName)
                    {
                        //ok
                    }
                    else
                    {
                        HttpContext.Session["GCEuUserSettings"] = null;
                        HttpContext.Session["GCEuUserSettingsForUser"] = null;
                        result = null;
                    }
                }
                if (result == null)
                {
                    //get it!
                    int? YafUserID = null;
                    using (PetaPoco.Database db = new PetaPoco.Database(dbYafForumConnString, "System.Data.SqlClient"))
                    {
                        var idl = db.Fetch<int?>("select UserID from Yaf_User where Name=@0", _orchardServices.WorkContext.CurrentUser.UserName);
                        if (idl != null && idl.Count > 0)
                        {
                            YafUserID = (int)idl[0];
                        }
                    }
                    if (YafUserID != null)
                    {
                        using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                        {
                            result = db.FirstOrDefault<GCEuUserSettings>("where YafUserID = @0", YafUserID);

                            if (result == null)
                            {
                                result = new GCEuUserSettings();
                                result.YafUserID = (int)YafUserID;
                                result.DefaultCountryCode = 141;
                                result.SortGeocachesBy = (int)GeocacheSearchFilterOrderOnItem.PublicationDate;
                                result.SortGeocachesDirection = -1;
                                result.NewestCachesMode = null;
                                db.Insert(result);
                            }
                            else if (result.GCComUserID != null)
                            {
                                var lapih = db.FirstOrDefault<GCEuLiveAPIHelpers>("where GCComUserID = @0", result.GCComUserID);
                                if (lapih == null)
                                {
                                    result.GCComUserID = null;
                                    result.LiveAPIToken = null;
                                    db.Update("GCEuUserSettings", "YafUserID", result);
                                }
                                else if (result.ShowGeocachesOnGlobal==null)
                                {
                                    result.ShowGeocachesOnGlobal = true;
                                }
                            }
                        }
                        HttpContext.Session["GCEuUserSettings"] = result;
                        HttpContext.Session["GCEuUserSettingsForUser"] = _orchardServices.WorkContext.CurrentUser.UserName;
                    }
                    setPM(result);
                }
            }
            else
            {
                result = HttpContext.Session["GCEuUserSettings"] as GCEuUserSettings;
                if (result != null)
                {
                    if (result.YafUserID > 1)
                    {
                        result = null;
                    }
                }
                if (result == null)
                {
                    result = new GCEuUserSettings();
                    result.YafUserID = 1;
                    result.DefaultCountryCode = 141;
                    HttpContext.Session["GCEuUserSettings"] = result;
                    HttpContext.Session["GCEuUserSettingsForUser"] = "";
                }
            }
            return result;
        }

        private void setPM(GCEuUserSettings settings)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                if (settings != null)
                {
                    settings.IsPM = false;
                    if (settings.GCComUserID != null)
                    {
                        var gcComUsr = db.FirstOrDefault<GCComUser>("where ID=@0", settings.GCComUserID);
                        if (gcComUsr != null)
                        {
                            settings.IsPM = gcComUsr.MemberTypeId > 1;
                        }
                    }
                }
            }
        }

        public void UpdateSettings(GCEuUserSettings settings)
        {
            if (_orchardServices.WorkContext.CurrentUser != null)
            {
                GCEuUserSettings currentSettings = GetSettings();
                if (currentSettings != null && currentSettings.YafUserID>1 && currentSettings.YafUserID == settings.YafUserID)
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                    {
                        if (settings.ShowGeocachesOnGlobal==null && settings.GCComUserID != null && !string.IsNullOrEmpty(settings.LiveAPIToken))
                        {
                            settings.ShowGeocachesOnGlobal = true;
                        }
                        db.Update("GCEuUserSettings", "YafUserID", settings);
                        try
                        {
                            if (settings.GCComUserID != null && !string.IsNullOrEmpty(settings.LiveAPIToken))
                            {
                                var m = new GCEuLiveAPIHelpers();
                                m.YafUserID = settings.YafUserID;
                                m.GCComUserID = (long)settings.GCComUserID;
                                m.LiveAPIToken = settings.LiveAPIToken;
                                db.Execute("delete from GCEuLiveAPIHelpers where YafUserID = @0", m.YafUserID);
                                if (m.YafUserID != 7)
                                {
                                    db.Insert(m);
                                }
                                else
                                {
                                    using (PetaPoco.Database db3 = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                                    {
                                        var gcComUsr = db3.FirstOrDefault<GCComUser>("where ID = @0", settings.GCComUserID);
                                        if (gcComUsr != null)
                                        {
                                            using (PetaPoco.Database db2 = new PetaPoco.Database(dbTaskSchedulerConnString, "System.Data.SqlClient"))
                                            {
                                                db2.Execute("update GcComAccounts set Token = @0 where Name = @1", m.LiveAPIToken, gcComUsr.UserName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                setPM(currentSettings);
            }
       }
    }
}
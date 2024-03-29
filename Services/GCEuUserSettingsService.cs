﻿using Globalcaching.Models;
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
        GCEuUserSettings GetSettings(long gcComUserID);
        void UpdateSettings(GCEuUserSettings settings);
        string GetEMail(int yafUserID);
        string GetName(int yafUserID);
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

        public string GetName(int yafUserID)
        {
            string result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbYafForumConnString, "System.Data.SqlClient"))
            {
                var idl = db.Fetch<string>("select Name from Yaf_User where UserID=@0", yafUserID);
                if (idl != null && idl.Count > 0)
                {
                    result = idl[0];
                }
            }
            return result;
        }

        public GCEuUserSettings GetSettings(long gcComUserID)
        {
            GCEuUserSettings result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.FirstOrDefault<GCEuUserSettings>("where GCComUserID = @0", gcComUserID);
            }
            if (result != null)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbYafForumConnString, "System.Data.SqlClient"))
                {
                    result.IsDonator = db.ExecuteScalar<int>("select count(1) from yaf_UserGroup where UserID=@0 and GroupID in (1, 4)", result.YafUserID) > 0;
                }
                validateLiveAPISettings(result);
            }
            return result;
        }

        public GCEuUserSettings GetSettings(string userName)
        {
            GCEuUserSettings result = null;
            int? YafUserID = null;
            bool isDonator = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbYafForumConnString, "System.Data.SqlClient"))
            {
                var idl = db.Fetch<int?>("select UserID from Yaf_User where Name=@0", userName);
                if (idl != null && idl.Count > 0)
                {
                    YafUserID = (int)idl[0];
                    isDonator = db.ExecuteScalar<int>("select count(1) from yaf_UserGroup where UserID=@0 and GroupID in (1, 4)", YafUserID) > 0;
                }
            }
            if (YafUserID != null)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    result = db.FirstOrDefault<GCEuUserSettings>("where YafUserID = @0", YafUserID);
                    if (result != null)
                    {
                        result.IsDonator = isDonator;
                    }
                }
            }
            if (result != null)
            {
                validateLiveAPISettings(result);
            }
            return result;
        }

        public GCEuUserSettings GetSettings()
        {
            GCEuUserSettings result = null;
            if (_orchardServices.WorkContext.CurrentUser != null)
            {
                result = HttpContext.Session["GCEuUserSettingsV2"] as GCEuUserSettings;
                if (result != null)
                {
                    //just to be sure, check it!
                    if (HttpContext.Session["GCEuUserSettingsForUserV2"] as string == _orchardServices.WorkContext.CurrentUser.UserName && result.ExpirationTime > DateTime.Now)
                    {
                        //ok
                    }
                    else
                    {
                        HttpContext.Session["GCEuUserSettingsV2"] = null;
                        HttpContext.Session["GCEuUserSettingsForUserV2"] = null;
                        result = null;
                    }
                }
                if (result == null)
                {
                    //get it!
                    int? YafUserID = null;
                    bool isDonator = false;
                    using (PetaPoco.Database db = new PetaPoco.Database(dbYafForumConnString, "System.Data.SqlClient"))
                    {
                        var idl = db.Fetch<int?>("select UserID from Yaf_User where Name=@0", _orchardServices.WorkContext.CurrentUser.UserName);
                        if (idl != null && idl.Count > 0)
                        {
                            YafUserID = (int)idl[0];
                            isDonator = db.ExecuteScalar<int>("select count(1) from yaf_UserGroup where UserID=@0 and GroupID in (1, 4)", YafUserID) > 0;
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
                                result.IsDonator = isDonator;
                                result.DefaultCountryCode = 141;
                                result.SortGeocachesBy = (int)GeocacheSearchFilterOrderOnItem.PublicationDate;
                                result.SortGeocachesDirection = -1;
                                result.NewestCachesMode = null;
                                db.Insert(result);
                            }
                            else if (result.GCComUserID != null)
                            {
                                result.IsDonator = isDonator;
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
                        validateLiveAPISettings(result);
                        HttpContext.Session["GCEuUserSettingsV2"] = result;
                        HttpContext.Session["GCEuUserSettingsForUserV2"] = _orchardServices.WorkContext.CurrentUser.UserName;
                    }
                    setPM(result);
                }
            }
            else
            {
                result = HttpContext.Session["GCEuUserSettingsV2"] as GCEuUserSettings;
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
                    result.IsDonator = false;
                    result.DefaultCountryCode = 141;
                    HttpContext.Session["GCEuUserSettingsV2"] = result;
                    HttpContext.Session["GCEuUserSettingsForUserV2"] = "";
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

        private void validateLiveAPISettings(GCEuUserSettings settings)
        {
            if (settings != null && settings.GCComUserID != null && settings.YafUserID > 1 && settings.YafUserID!=7)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    if (db.FirstOrDefault<GCEuLiveAPIHelpers>("WHERE YafUserID=@0", settings.YafUserID) == null)
                    {
                        settings.GCComUserID = null;
                        settings.LiveAPIToken = null;
                        db.Update("GCEuUserSettings", "YafUserID", settings);
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
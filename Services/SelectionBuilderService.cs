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
    public interface ISelectionBuilderService: IDependency
    {
        SelectionBuilderModel GetOwnSelectionGraphs(int selectId);
        SelectionBuilderModel SaveGraph(GCEuSelectionBuilder m);
        SelectionBuilderModel DeleteGraph(int id);
    }

    public class SelectionBuilderService : ISelectionBuilderService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public SelectionBuilderService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public SelectionBuilderModel DeleteGraph(int id)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    db.Execute("delete from GCEuSelectionBuilder where UserID=@0 and ID=@1", settings.YafUserID, id);
                }
            }
            return GetOwnSelectionGraphs(0);
        }

        public SelectionBuilderModel SaveGraph(GCEuSelectionBuilder m)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 1)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    m.UserID = settings.YafUserID;
                    m.RecentEdit = DateTime.Now;
                    if (m.ID <= 0)
                    {
                        m.ID = 0;
                        db.Insert(m);
                    }
                    else
                    {
                        if (db.FirstOrDefault<GCEuSelectionBuilder>("where UserID=@0 and ID=@1", settings.YafUserID, m.ID) != null)
                        {
                            db.Save(m);
                        }
                    }
                }
            }
            return GetOwnSelectionGraphs(m.ID);
        }

        public SelectionBuilderModel GetOwnSelectionGraphs(int selectId)
        {
            SelectionBuilderModel result = new SelectionBuilderModel();
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID>1)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    result.AllOwnedGraphs = db.Fetch<GCEuSelectionBuilder>("where UserID=@0", settings.YafUserID);
                }
                result.ActiveGraph = (from a in result.AllOwnedGraphs where a.ID == selectId select a).FirstOrDefault();
                if (result.ActiveGraph == null && result.AllOwnedGraphs.Count > 0)
                {
                    result.ActiveGraph = result.AllOwnedGraphs[0];
                }
                foreach (var g in result.AllOwnedGraphs)
                {
                    if (g != result.ActiveGraph)
                    {
                        g.Graph = "";
                    }
                }
            }
            return result;
        }

    }
}
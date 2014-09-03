using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGCComSearchGeocacheLogsService : IDependency
    {
        GCComGeocacheLogOfUserSearchResult GetGeocachingComLogsOfUser(int page, int pageSize, string name);
    }

    public class GCComSearchGeocacheLogsService : IGCComSearchGeocacheLogsService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public readonly IGCComSearchUserService _gcComSearchUserService;

        public GCComSearchGeocacheLogsService(IGCComSearchUserService gcComSearchUserService)
        {
            _gcComSearchUserService = gcComSearchUserService;
        }

        public GCComGeocacheLogOfUserSearchResult GetGeocachingComLogsOfUser(int page, int pageSize, string name)
        {
            GCComGeocacheLogOfUserSearchResult result = new GCComGeocacheLogOfUserSearchResult();
            result.User = _gcComSearchUserService.GetGeocachingComUser(name);
            result.PageCount = 1;
            result.CurrentPage = 1;
            if (result.User != null)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                {
                    var items = db.Page<GCComGeocacheLogEx>(page, pageSize, "select GCComGeocacheLog.*, GCComGeocache.GeocacheTypeId, GCComGeocache.Name, GCComGeocache.Url as GeocacheUrl from GCComGeocacheLog inner join GCComGeocache on GCComGeocache.ID=GCComGeocacheLog.GeocacheID Where FinderId=@0 order by VisitDate desc, GCComGeocacheLog.ID desc", result.User.ID);
                    result.Logs = items.Items.ToArray();
                    result.CurrentPage = items.CurrentPage;
                    result.PageCount = items.TotalPages;
                    result.TotalCount = items.TotalItems;
                }
            }
            else
            {
                result.Logs = new GCComGeocacheLogEx[0];
                result.TotalCount = 0;
            }
            return result;
        }
    }
}
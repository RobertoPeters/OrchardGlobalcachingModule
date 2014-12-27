using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGCComSearchUserService : IDependency
    {
        GCComUser GetGeocachingComUser(string name);
        GCComUser GetGeocachingComUser(long userId);
        GCComUserSearchResult GetGeocachingComUsers(int page, int pageSize, string name);
    }

    public class GCComSearchUserService : IGCComSearchUserService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public GCComUser GetGeocachingComUser(string name)
        {
            GCComUser result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = db.FirstOrDefault<GCComUser>("where UserName=@0", name);
            }
            return result;
        }

        public GCComUser GetGeocachingComUser(long userId)
        {
            GCComUser result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                result = db.FirstOrDefault<GCComUser>("where ID=@0", userId);
            }
            return result;
        }

        public GCComUserSearchResult GetGeocachingComUsers(int page, int pageSize, string name)
        {
            GCComUserSearchResult result = new GCComUserSearchResult();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<GCComUser>(page, pageSize, string.Format("Where UserName like '%{0}%' order by UserName", (name ?? "").Replace("'", "''")));
                result.Users = items.Items.ToArray();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }
    }
}
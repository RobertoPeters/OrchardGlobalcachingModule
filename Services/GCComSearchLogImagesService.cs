using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGCComSearchLogImagesService : IDependency
    {
        GCComSearchLogImagesResult GetLogImages(GCComSearchLogImagesFilter filter, int page, int pageSize);
    }

    public class GCComSearchLogImagesService : IGCComSearchLogImagesService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public GCComSearchLogImagesResult GetLogImages(GCComSearchLogImagesFilter filter, int page, int pageSize)
        {
            GCComSearchLogImagesResult result = new GCComSearchLogImagesResult();
            result.Filter = filter;
            result.PageCount = 1;
            result.CurrentPage = 1;
            filter.StartDate = filter.StartDate.Date;
            filter.EndDate = filter.EndDate.Date.AddHours(23);
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder.Append("select GCComGeocacheLogImage.Name as Name, GCComGeocacheLogImage.ThumbUrl, GCComGeocacheLogImage.Url as ImageUrl, GCComGeocacheLog.Url as logUrl, GCComGeocacheLog.VisitDate, GCComUser.UserName, GCComUser.PublicGuid as UserNameGuid, GCComGeocache.Name as GeocacheName, GCComGeocache.Code as GeocacheCode")
                    .From("GCComGeocacheLogImage with (nolock)")
                    .InnerJoin("GCComGeocacheLog with (nolock)").On("GCComGeocacheLogImage.LogID=GCComGeocacheLog.ID")
                    .InnerJoin("GCComGeocache with (nolock)").On("GCComGeocacheLog.GeocacheID=GCComGeocache.ID")
                    .InnerJoin("GCComUser with (nolock)").On("GCComGeocacheLog.FinderId=GCComUser.ID")
                    .Where("GCComGeocacheLog.VisitDate>=@0 and GCComGeocacheLog.VisitDate<=@1", filter.StartDate, filter.EndDate);
                if (filter.CountryID > 0)
                {
                    sql = sql.Append("and GCComGeocache.CountryID=@0", filter.CountryID);
                }
                sql = sql.OrderBy("GCComGeocacheLog.VisitDate", "GCComGeocacheLog.ID");

                var items = db.Page<GCComSearchLogImageItem>(page, pageSize, sql);
                result.Items = items.Items.ToArray();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }
    }
}
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
    public interface IBookmarksService : IDependency
    {
        BookmarkModel GetBookmarks(int page, int pageSize, string ListName, string UserName, int NumberOfItems, int NumberOfKnownItems);
    }

    public class BookmarksService : IBookmarksService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public BookmarksService()
        {
        }

        public BookmarkModel GetBookmarks(int page, int pageSize, string ListName, string UserName, int NumberOfItems, int NumberOfKnownItems)
        {
            var result = new BookmarkModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Filter = new BookmarkFilter()
            {
                ListName = ListName,
                NumberOfItems = NumberOfItems,
                NumberOfKnownItems = NumberOfKnownItems,
                UserName = UserName
            };
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                var sql = PetaPoco.Sql.Builder.Append("select GCComBookmark.ListID, GCComBookmark.GCComUserID, GCComBookmark.ListDescription, GCComBookmark.ListGUID, GCComBookmark.ListName, GCComBookmark.NumberOfItems, GCComBookmark.NumberOfKnownItems, GCComUser.AvatarUrl, GCComUser.PublicGuid, GCComUser.UserName from GCComBookmark with (nolock) inner join GCComUser with (nolock) on GCComBookmark.GCComUserID=GCComUser.ID")
                    .Append("where GCComBookmark.NumberOfItems>=@0", NumberOfItems)
                    .Append(" and GCComBookmark.NumberOfKnownItems>=@0", NumberOfKnownItems);
                if (!string.IsNullOrEmpty(ListName))
                {
                    sql.Append(string.Format(" and GCComBookmark.ListName like '%{0}%'", ListName.Replace("'","''")));
                }
                if (!string.IsNullOrEmpty(UserName))
                {
                    sql.Append(string.Format(" and GCComUser.UserName like '%{0}%'", UserName.Replace("'", "''")));
                }
                var items = db.Page<BookmarkItem>(page, pageSize, sql);
                result.Bookmarks = items.Items;
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }

    }
}
using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class BookmarkItem
    {
        public long ListID { get; set; }
        public long GCComUserID { get; set; }
        public string ListDescription { get; set; }
        public Guid ListGUID { get; set; }
        public string ListName { get; set; }
        public int NumberOfItems { get; set; }
        public int? NumberOfKnownItems { get; set; } //items we also have in our database

        public string AvatarUrl { get; set; }
        public Guid PublicGuid { get; set; }
        public string UserName { get; set; }
    }

    public class BookmarkFilter
    {
        public string ListName { get; set; }
        public string UserName { get; set; }
        public int NumberOfItems { get; set; }
        public int NumberOfKnownItems { get; set; }
    }

    public class BookmarkModel
    {
        public BookmarkFilter Filter { get; set; }
        public List<BookmarkItem> Bookmarks { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
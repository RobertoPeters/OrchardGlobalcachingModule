using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class GCComSearchLogImageItem
    {
        public string ThumbUrl { get; set; }
        public string ImageUrl { get; set; }
        public string LogUrl { get; set; }
        public string Name { get; set; }
        public DateTime VisitDate { get; set; }
        public string UserName { get; set; }
        public Guid UserNameGuid { get; set; }
        public string GeocacheName { get; set; }
        public string GeocacheCode { get; set; }
    }

    public class GCComSearchLogImagesResult
    {
        public GCComSearchLogImagesFilter Filter { get; set; }
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public long CurrentPage { get; set; }
        public GCComSearchLogImageItem[] Items { get; set; }
    }
}
using Globalcaching.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Handlers
{
    public class ShopArticleHandler : ContentHandler {
        public ShopArticleHandler(IRepository<ShopArticleRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
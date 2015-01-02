using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class YafMostRecentContentModel
    {
        public IEnumerable<YafPost> YafPosts { get; set; }
        public IEnumerable<YafShout> YafShouts { get; set; }
    }
}
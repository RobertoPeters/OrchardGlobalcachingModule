using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class CombiRankingItem
    {
        public int Ranking { get; set; }
        public int FoundsPoints { get; set; }
        public int FTFPoints { get; set; }

        public string AvatarUrl { get; set; }
        public int? FindCount { get; set; }
        public Guid PublicGuid { get; set; }
        public string UserName { get; set; }
        public long MemberTypeId { get; set; }
    }

    public class CombiRankingModel
    {
        public List<CombiRankingItem> Items { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }

        public int RankYear { get; set; }
        public int RankType { get; set; }
        public string NameFilter { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class FTFStatsInfo
    {
        public long UserID { get; set; }
        public int? Jaar { get; set; }
        public int FTFCount { get; set; }
        public int STFCount { get; set; }
        public int TTFCount { get; set; }
        public int Position { get; set; }
        public int PositionPoints { get; set; }

        public string AvatarUrl { get; set; }
        public int? FindCount { get; set; }
        public Guid PublicGuid { get; set; }
        public string UserName { get; set; }

    }

    public class FTFStatsModel
    {
        public string UserName { get; set; }
        public int Jaar { get; set; } //0 = all
        public int RankingType { get; set; } //0 = points, 1 = ftf

        public List<FTFStatsInfo> FTFInfo { get; set; }
        public long CurrentPage { get; set; }
        public long PageCount { get; set; }
        public long TotalCount { get; set; }
    }
}
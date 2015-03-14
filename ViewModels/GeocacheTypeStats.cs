using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheTypeStats
    {
        public long GeocacheTypeId { get; set; }
        public string GeocacheTypeName { get; set; }
        public int CacheCount { get; set; }
        public int FoundCount { get; set; }
        public int PMFoundCount { get; set; }
        public int LogImageCount { get; set; }
        public int FavoritePoints { get; set; }
        public int DaysOnline { get; set; }
    }
}
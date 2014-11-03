using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class LiveAPIDownloadStatus
    {
        public int UserID { get; set; }
        public int TotalToDownload { get; set; }
        public int Downloaded { get; set; }
        public bool Canceled { get; set; }
        public string StatusText { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishedTime { get; set; }

        [PetaPoco.Ignore]
        public bool IsDownloading { get; set; }

    }
}
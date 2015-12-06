using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComBookmark")]
    public class GCComBookmark
    {
        public long ListID { get; set; }
        public long GCComUserID { get; set; }
        public string ListDescription { get; set; }
        public Guid ListGUID { get; set; }
        public bool ListIsArchived { get; set; }
        public bool ListIsPublic { get; set; }
        public bool ListIsShared { get; set; }
        public bool ListIsSpecial { get; set; }
        public string ListName { get; set; }
        public int ListTypeID { get; set; }
        public int NumberOfItems { get; set; }
        public int? NumberOfKnownItems { get; set; } //items we also have in our database
    }
}
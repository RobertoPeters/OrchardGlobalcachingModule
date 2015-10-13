using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuComUserNameChange")]
    public class GCEuComUserNameChange
    {
        public long ID { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
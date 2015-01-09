using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuTrackable")]
    public class GCEuTrackable
    {
        public string Code { get; set; }
        public int GroupID { get; set; }
        public DateTime? Updated { get; set; }
    }
}
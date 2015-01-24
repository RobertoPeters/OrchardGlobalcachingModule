using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacheBatchLogModel
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        [PetaPoco.Ignore]
        public bool Found { get; set; }
    }
}
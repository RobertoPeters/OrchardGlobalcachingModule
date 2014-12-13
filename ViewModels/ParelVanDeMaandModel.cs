using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class ParelVanDeMaandInfo
    {
        public string Code {get; set;}
        public string Name { get; set; }
        public string GeocacheTypeName { get; set; }
        public DateTime PublishedAtDate { get; set; }

        [PetaPoco.Ignore]
        public string BannerUrl { get; set; }
        [PetaPoco.Ignore]
        public string Month { get; set; }
    }
    public class ParelVanDeMaandModel
    {
        public ParelVanDeMaandInfo Netherlands { get; set; }
        public ParelVanDeMaandInfo Belgium { get; set; }
    }
}
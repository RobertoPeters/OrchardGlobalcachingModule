using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class SearchByAttributesModel
    {
        public List<GCComGeocacheType> GeocacheTypes { get; set; }
        public List<int> Containers { get; set; }
    }
}
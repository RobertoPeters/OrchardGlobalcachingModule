using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class SelectionBuilderModel
    {
        public GCEuSelectionBuilder ActiveGraph { get; set; }
        public List<GCEuSelectionBuilder> AllOwnedGraphs { get; set; }
        public GCEuUserSettings UserSettings { get; set; }
        public List<GCComGeocacheType> GeocacheTypes { get; set; }
        public List<GCComAttributeType> AttributeTypes { get; set; }
        public List<int> Containers { get; set; }
    }
}
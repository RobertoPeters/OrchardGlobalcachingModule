using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{

    public class MacroFunctionInfo
    {
        public string Name { get; set; }
        public string ProtoType { get; set; }
        public string Description { get; set; }
        public string Examples { get; set; }
        public bool PMOnly { get; set; }
    }

    public class GeocacheFilterMacroModel
    {
        public MacroFunctionInfo[] Functions { get; set; }
        public List<GCEuGeocacheFilterMacro> Macros { get; set; }
    }
}
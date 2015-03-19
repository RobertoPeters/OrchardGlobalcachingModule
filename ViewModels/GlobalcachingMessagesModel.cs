using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GlobalcachingMessagesModel
    {
        public List<string> ErrorMessages { get; set; }
        public List<string> WarningMessages { get; set; }
        public List<string> InformationMessages { get; set; }
        public GCEuUserSettings UserSettings { get; set; }
    }
}
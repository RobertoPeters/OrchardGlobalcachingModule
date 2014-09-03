using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class EditCCCSettingsModel
    {
        public GCEuUserSettings UserSettings { get; set; }
        public GCEuCCCUser CCCSettings { get; set; }
        public string ReturnUrl { get; set; }
    }
}
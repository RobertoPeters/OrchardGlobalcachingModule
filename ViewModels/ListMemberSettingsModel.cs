using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class ListMemberSettingsModel : GCEuUserSettings
    {
        public string Name { get; set; }
    }
}
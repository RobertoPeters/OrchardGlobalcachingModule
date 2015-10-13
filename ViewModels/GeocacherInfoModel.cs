using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class GeocacherInfoModel
    {
        public GCComUser GCComUser { get; set; }
        public GCEuUserSettings GCEuUserSettings { get; set; }
        public string GCEuUserName { get; set; }
        public List<GCEuComUserNameChange> GCComNameChanges { get; set; }
    }
}
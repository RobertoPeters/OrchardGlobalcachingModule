using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class ListCCCMembersModel : GCEuCCCUser
    {
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
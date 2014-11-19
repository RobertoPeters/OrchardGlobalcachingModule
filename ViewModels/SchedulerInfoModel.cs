using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class SchedulerInfoModel
    {
        public SchedulerStatus Scheduler { get; set; }
        public List<ServiceInfo> Services { get; set; }
        public List<GcComAccounts> ServiceAccounts { get; set; }
    }
}
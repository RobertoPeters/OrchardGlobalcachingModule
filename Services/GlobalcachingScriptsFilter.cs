using Orchard.Mvc.Filters;
using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Services
{
    public class GlobalcachingScriptsFilter : FilterProvider, IResultFilter
    {
        private readonly IResourceManager _resourceManager;

        public GlobalcachingScriptsFilter(IResourceManager resourceManager)
        {
                _resourceManager = resourceManager;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {

            // Should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
                return;

            // Include the following script on all pages
            _resourceManager.Require("script", "GlobalcachingPager.Script").AtHead();
            _resourceManager.Require("script", "GlobalcachingBusyWaitDlg.Script").AtFoot();
            _resourceManager.Require("script", "moment.Script").AtFoot();
            _resourceManager.Require("script", "bootstrap-datetimepicker.Script").AtFoot();
            _resourceManager.Require("script", "bootstrap-datetimepicker-nl.Script").AtFoot();
            _resourceManager.Require("stylesheet", "usersonline.Style").AtHead();
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }
    }
}
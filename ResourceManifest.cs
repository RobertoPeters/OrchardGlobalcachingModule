using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("GlobalcachingPager.Script").SetUrl("GlobalcachingPager-2.js").SetDependencies("jQuery");
            manifest.DefineScript("GlobalcachingBusyWaitDlg.Script").SetUrl("GlobalcachingBusyWaitDlg.js").SetDependencies("jQuery");
            manifest.DefineScript("moment.Script").SetUrl("moment.js").SetDependencies("jQuery");
            manifest.DefineScript("bootstrap-datetimepicker.Script").SetUrl("bootstrap-datetimepicker.js").SetDependencies("jQuery");
            manifest.DefineScript("bootstrap-datetimepicker-nl.Script").SetUrl("bootstrap-datetimepicker.nl.js").SetDependencies("jQuery");
            manifest.DefineStyle("datetimepicker.Style").SetUrl("datetimepicker.css");
            manifest.DefineScript("keydragzoom_packed.Script").SetUrl("keydragzoom_packed.js");
            manifest.DefineScript("markerclusterer_compiled.Script").SetUrl("markerclusterer_compiled.js");
            manifest.DefineScript("gmapv3.Script").SetUrl("gmapv3-2.js");
            manifest.DefineStyle("usersonline.Style").SetUrl("usersonline.css");
            manifest.DefineStyle("zabuto_calendar.Style").SetUrl("zabuto_calendar.min.css");
            manifest.DefineScript("zabuto_calendar.Script").SetUrl("zabuto_calendar.min.js");
            manifest.DefineScript("GoogleAnalytics.Script").SetUrl("GoogleAnalytics.js");
            manifest.DefineScript("lodash.Script").SetUrl("lodash.min.js");
            manifest.DefineScript("Backbone.Script").SetUrl("backbone-min.js").SetDependencies("lodash.Script");
            //manifest.DefineScript("JointJS.Script").SetUrl("joint.nojquery.min.js");
            manifest.DefineScript("JointJS.Script").SetUrl("joint.min096.js").SetDependencies("Backbone.Script");
            //manifest.DefineStyle("JointJS.Style").SetUrl("joint.nojquery.min.css");
            manifest.DefineStyle("JointJS.Style").SetUrl("joint.min096.css");
            //manifest.DefineScript("JointJSShapes.Script").SetUrl("joint.shapes.devs.min.js");
            manifest.DefineScript("JointJSShapes.Script").SetUrl("joint.shapes.devs.min096.js");
            manifest.DefineScript("JointJSShapes.Script").SetUrl("joint.shapes.devs.min096.js");
            manifest.DefineStyle("BootSideMenu.Style").SetUrl("BootSideMenu.css");
            manifest.DefineScript("BootSideMenu.Script").SetUrl("BootSideMenu.js");
        }
    }
}
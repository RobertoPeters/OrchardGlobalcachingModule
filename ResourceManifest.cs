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

            manifest.DefineScript("GlobalcachingPager.Script").SetUrl("GlobalcachingPager.js").SetDependencies("jQuery");
            manifest.DefineScript("GlobalcachingBusyWaitDlg.Script").SetUrl("GlobalcachingBusyWaitDlg.js").SetDependencies("jQuery");
            manifest.DefineScript("moment.Script").SetUrl("moment.js").SetDependencies("jQuery");
            manifest.DefineScript("bootstrap-datetimepicker.Script").SetUrl("bootstrap-datetimepicker.js").SetDependencies("jQuery");
            manifest.DefineScript("bootstrap-datetimepicker-nl.Script").SetUrl("bootstrap-datetimepicker.nl.js").SetDependencies("jQuery");
            manifest.DefineStyle("datetimepicker.Style").SetUrl("datetimepicker.css");
            manifest.DefineScript("keydragzoom_packed.Script").SetUrl("keydragzoom_packed.js");
            manifest.DefineScript("markerclusterer_compiled.Script").SetUrl("markerclusterer_compiled.js");
            manifest.DefineScript("gmapv3.Script").SetUrl("gmapv3.js");
            manifest.DefineStyle("usersonline.Style").SetUrl("usersonline.css");
        }
    }
}
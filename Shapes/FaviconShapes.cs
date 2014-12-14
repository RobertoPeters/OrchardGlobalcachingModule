using Orchard;
using Orchard.DisplayManagement.Descriptors;
using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Shapes
{
    public class FaviconShapes : IShapeTableProvider
    {
        private readonly IWorkContextAccessor _wca;

        public FaviconShapes(IWorkContextAccessor wca)
        {
            _wca = wca;
        }

        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("HeadLinks")
                .OnDisplaying(shapeDisplayingContext =>
                {
                    string faviconUrl = VirtualPathUtility.ToAbsolute("~/Modules/Globalcaching/Media/gcicon.ico");
                    if (!string.IsNullOrWhiteSpace(faviconUrl))
                    {
                        // Get the current favicon from head
                        var resourceManager = _wca.GetContext().Resolve<IResourceManager>();
                        var links = resourceManager.GetRegisteredLinks();
                        var currentFavicon = links
                            .Where(l => l.Rel == "shortcut icon" && l.Type == "image/x-icon")
                            .FirstOrDefault();
                        // Modify if found
                        if (currentFavicon != default(LinkEntry))
                        {
                            currentFavicon.Href = faviconUrl;
                        }
                        else
                        {
                            // Add the new one
                            resourceManager.RegisterLink(new LinkEntry
                            {
                                Type = "image/x-icon",
                                Rel = "shortcut icon",
                                Href = faviconUrl
                            });
                        }
                    }
                });
        }
    }
}
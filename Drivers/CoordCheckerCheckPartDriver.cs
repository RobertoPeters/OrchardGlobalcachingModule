using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class CoordCheckerCheckPartDriver : ContentPartDriver<CoordCheckerCheckPart>
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        protected override string Prefix { get { return ""; } }

        public CoordCheckerCheckPartDriver(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        protected override DriverResult Display(CoordCheckerCheckPart part, string displayType, dynamic shapeHelper)
        {
            CoordCheckerCheckModel m = new CoordCheckerCheckModel();
            string wp = HttpContext.Request.QueryString["wp"];
            if (!string.IsNullOrEmpty(wp))
            {
                m.Code = wp;
            }
            return ContentShape("Parts_CoordCheckerCheck",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.CoordCheckerCheck",
                            Model: m,
                            Prefix: Prefix));
        }

    }
}
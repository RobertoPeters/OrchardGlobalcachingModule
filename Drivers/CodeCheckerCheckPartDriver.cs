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
    public class CodeCheckerCheckPartDriver : ContentPartDriver<CodeCheckerCheckPart>
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        protected override string Prefix { get { return ""; } }

        public CodeCheckerCheckPartDriver(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        protected override DriverResult Display(CodeCheckerCheckPart part, string displayType, dynamic shapeHelper)
        {
            CodeCheckerCheckModel m = new CodeCheckerCheckModel();
            string code = HttpContext.Request.QueryString["code"];
            string id = HttpContext.Request.QueryString["id"];
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(id))
            {
                m.Code = code;
                m.ID = int.Parse(id);
            }
            return ContentShape("Parts_CodeCheckerCheck",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.CodeCheckerCheck",
                            Model: m,
                            Prefix: Prefix));
        }

    }
}
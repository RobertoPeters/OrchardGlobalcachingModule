using Globalcaching.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class LogCorrectionPartDriver : ContentPartDriver<LogCorrectionPart>
    {
        public LogCorrectionPartDriver()
        {
        }

        protected override DriverResult Display(LogCorrectionPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_LogCorrection",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.LogCorrection",
                            Model: null,
                            Prefix: Prefix));
        }
    }

}
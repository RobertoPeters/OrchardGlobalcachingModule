using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class FinancialYearStatusDriver : ContentPartDriver<FinancialYearStatusPart>
    {
        private readonly IFinancialYearStatusService _financialYearStatusService;

        public FinancialYearStatusDriver(IFinancialYearStatusService financialYearStatusService)
        {
            _financialYearStatusService = financialYearStatusService;
        }

        protected override DriverResult Display(FinancialYearStatusPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FinancialYearStatus",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.FinancialYearStatus",
                            Model: _financialYearStatusService.GetFinancialYearStatus(part),
                            Prefix: Prefix));
        }

        protected override DriverResult Editor(FinancialYearStatusPart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_FinancialYearStatus_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/FinancialYearStatus",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(FinancialYearStatusPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }

}
using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class SelectionBuilderPartDriver : ContentPartDriver<SelectionBuilderPart>
    {
        private ISelectionBuilderService _selectionBuilderService;

        public SelectionBuilderPartDriver(ISelectionBuilderService selectionBuilderService)
        {
            _selectionBuilderService = selectionBuilderService;
        }

        protected override DriverResult Display(SelectionBuilderPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_SelectionBuilder",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.SelectionBuilder",
                            Model: _selectionBuilderService.GetOwnSelectionGraphs(0),
                            Prefix: Prefix));
        }
    }

}
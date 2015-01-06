using Globalcaching.Models;
using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class SelectionBuilderController: Controller
    {
        private ISelectionBuilderService _selectionBuilderService;

        public SelectionBuilderController(ISelectionBuilderService selectionBuilderService)
        {
            _selectionBuilderService = selectionBuilderService;
        }

        public ActionResult SaveGraph(string id, string name, string ispublic, string comment, string graph)
        {
            GCEuSelectionBuilder m =new GCEuSelectionBuilder();
            m.Comment = comment;
            m.Graph = graph;
            m.ID = int.Parse(id);
            m.IsPublic = bool.Parse(ispublic);
            m.Name = name;
            m.RecentEdit = DateTime.Now;
            return Json(_selectionBuilderService.SaveGraph(m));
        }

        public ActionResult LoadGraph(string id)
        {
            return Json(_selectionBuilderService.GetOwnSelectionGraphs(int.Parse(id)));
        }

        public ActionResult DeleteGraph(string id)
        {
            return Json(_selectionBuilderService.DeleteGraph(int.Parse(id)));
        }

    }
}
using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class MacroController : Controller
    {
        private readonly IMacroService _macroService;

        public MacroController(IMacroService macroService)
        {
            _macroService = macroService;
        }

        [HttpPost]
        public ActionResult Save()
        {
            return Json(_macroService.SaveMacro(Request["id"] as string, Request["macro"] as string));
        }

        [HttpPost]
        public ActionResult Run()
        {
            return Json(_macroService.SaveAndRunMacro(Request["id"] as string, Request["macro"] as string));
        }

        [HttpPost]
        public ActionResult GetMacro()
        {
            return Json(_macroService.GetMacro(long.Parse(Request["id"] as string)));
        }

        [HttpPost]
        public ActionResult DeleteMacro()
        {
            return Json(_macroService.DeleteMacro(long.Parse(Request["id"] as string)));
        }

    }
}
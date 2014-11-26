using Globalcaching.Models;
using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class GCComSearchLogImagesController : Controller
    {
        private readonly IGCComSearchLogImagesService _gcComSearchLogImagesService;

        public GCComSearchLogImagesController(IGCComSearchLogImagesService gcComSearchLogImagesService)
        {
            _gcComSearchLogImagesService = gcComSearchLogImagesService;
        }

        [HttpPost]
        public ActionResult Index()
        {
            GCComSearchLogImagesFilter filter = new GCComSearchLogImagesFilter();
            int page = 1;
            int pageSize = 30;
            DateTime StartDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            int CountryID = 0;
            int.TryParse(Request["page"] ?? "1", out page);
            int.TryParse(Request["pageSize"] ?? "30", out pageSize);
            int.TryParse(Request["CountryID"] ?? "0", out CountryID);
            DateTime.TryParse(Request["StartDate"] ?? DateTime.Now.ToString(), out StartDate);
            DateTime.TryParse(Request["EndDate"] ?? DateTime.Now.ToString(), out EndDate);
            if ((EndDate - StartDate).TotalDays > 7)
            {
                EndDate = StartDate.AddDays(7);
            }
            filter.CountryID = CountryID;
            filter.StartDate = StartDate;
            filter.EndDate = EndDate;
            return Json(_gcComSearchLogImagesService.GetLogImages(filter, page, pageSize));
        }
    }
}
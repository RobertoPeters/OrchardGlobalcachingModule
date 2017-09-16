using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class ForumController: Controller
    {
        private static string ForumBaseUrl = ConfigurationManager.AppSettings["forumBaseUrl"].ToString();

        public ForumController()
        {
        }

        public ActionResult Index(string pathInfo)
        {
            int pos = Request.RawUrl.IndexOf("forum/", StringComparison.InvariantCultureIgnoreCase);
            if (pos > 0)
            {
                return Redirect(string.Format("{0}{1}", ForumBaseUrl, Request.RawUrl.Substring(pos+6)));
            }
            else
            {
                return Redirect(ForumBaseUrl);
            }
        }
    }
}
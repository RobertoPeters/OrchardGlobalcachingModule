using Globalcaching.Core;
using Orchard;
using Orchard.Security;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class GAPPInfoController: Controller
    {
        public IOrchardServices Services { get; set; }

        public GAPPInfoController(IOrchardServices services)
        {
            Services = services;
        }


        public class GAPPAuthorizationPoco
        {
            public int Row { get; set; }
            public DateTime requestdate { get; set; }
            public string Username { get; set; }
            public int MemberType { get; set; }
            public long MemberId { get; set; }
        }
        [Themed]
        public ActionResult ListAuthorizations()
        {
            if (Services.Authorizer.Authorize(Permissions.GlobalAdmin))
            {
                List<GAPPAuthorizationPoco> m;
                using (PetaPoco.Database db = new PetaPoco.Database(DBCon.dbForumConnString, "System.Data.SqlClient"))
                {
                    m = db.Fetch<GAPPAuthorizationPoco>("SELECT ROW_NUMBER() OVER(ORDER BY requestdate DESC) AS Row, * from GAPPAuthorizations");
                }
                return View("Home", m);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

    }
}
using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class CodeCheckerController: Controller
    {
        private ICodeCheckerService _codeCheckerService;

        public CodeCheckerController(ICodeCheckerService codeCheckerService)
        {
            _codeCheckerService = codeCheckerService;
        }

        //[Themed]
        public ActionResult RedirectFromOldSite()
        {
            return Redirect(Request.Url.ToString().ToLower().Replace("caches/codecheck.aspx", "CodeChecker"));
        }

        [HttpPost]
        public ActionResult DeleteCode(string code)
        {
            return Json(_codeCheckerService.DeleteCode(int.Parse(code), 1, 20));
        }

        [HttpPost]
        public ActionResult GetCode(string code)
        {
            return Json(_codeCheckerService.CodeCheckerMaintModel(int.Parse(code), 1, 20));
        }

        [HttpPost]
        public ActionResult CheckCode(string id, string publiccode, string answer)
        {
            string remarks;
            bool ok = _codeCheckerService.CheckCode(int.Parse(id), publiccode, answer, out remarks);
            return Json(new { OK = ok, Remarks = remarks });
        }

        [HttpPost]
        public ActionResult GetAttempts(string id, int page, int pageSize)
        {
            return Json(_codeCheckerService.CodeCheckerAttempts(int.Parse(id), page, pageSize));
        }

        [HttpPost]
        public ActionResult CreateCode(string code, string answer, string answertext, string delaysec, string casesens, string gccomreq, string notiwrong, string noticorrect)
        {
            if (!string.IsNullOrEmpty(code) && code.Trim().Length > 0)
            {
                GCEuCodeCheckCode m = new GCEuCodeCheckCode();
                m.AnswerCode = answer.Trim();
                m.EmailNotifyOnFail = bool.Parse(notiwrong);
                m.EmailNotifyOnSuccess = bool.Parse(noticorrect);
                m.CaseSensative = bool.Parse(casesens);
                m.AnswerText = answertext;
                m.DelaySeconds = int.Parse(delaysec);
                m.GroundspeakAuthReq = bool.Parse(gccomreq);
                m.PublicCode = code;
                var result = _codeCheckerService.CreateCode(m, 1, 20);
                if (result != null)
                {
                    return Json(result);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdateCode(string id, string code, string answer, string answertext, string delaysec, string casesens, string gccomreq, string notiwrong, string noticorrect)
        {
            if (!string.IsNullOrEmpty(code) && code.Trim().Length > 0)
            {
                GCEuCodeCheckCode m = new GCEuCodeCheckCode();
                m.ID = int.Parse(id);
                m.AnswerCode = answer.Trim();
                m.EmailNotifyOnFail = bool.Parse(notiwrong);
                m.EmailNotifyOnSuccess = bool.Parse(noticorrect);
                m.CaseSensative = bool.Parse(casesens);
                m.AnswerText = answertext;
                m.DelaySeconds = int.Parse(delaysec);
                m.GroundspeakAuthReq = bool.Parse(gccomreq);
                m.PublicCode = code;
                var result = _codeCheckerService.UpdateCode(m, 1, 20);
                if (result != null)
                {
                    return Json(result);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
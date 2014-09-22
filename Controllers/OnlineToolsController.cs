using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class OnlineToolsController : Controller
    {
        public OnlineToolsController()
        {
        }

        public ActionResult Rot13(string id)
        {
            return Content(Core.Helper.DecryptHint(id));
        }

        public ActionResult WoordWaarde(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(string.Format("{1} a=1 ... z=26: {0}", wordvalue(id, 1), "Woordwaarde"));
            sb.AppendFormat(string.Format("\n{1} a=0 ... z=25: {0}", wordvalue(id, 0), "Woordwaarde"));
            sb.AppendFormat(string.Format("\n{1} z=1 ... a=26: {0}", wordvalueReverse(id, 1), "Woordwaarde"));
            sb.AppendFormat(string.Format("\n{1} z=0 ... a=25: {0}", wordvalueReverse(id, 0), "Woordwaarde"));
            sb.AppendFormat(string.Format("\n{1} {0}", wordvalueCharacterCount(id, true), "Woordwaarde met spaties"));
            sb.AppendFormat(string.Format("\n{1} {0}", wordvalueCharacterCount(id, false), "Woordwaarde zonder spaties"));
            return Content(sb.ToString());
        }

        public ActionResult ASCIIConvert(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========= ASCII (DEC) ==========");
            sb.AppendLine(ascii(id, true));
            sb.AppendLine("========= ASCII (HEX) ==========");
            sb.AppendLine(ascii(id, false));
            return Content(sb.ToString());
        }

        public string ascii(string s, bool decimaal)
        {
            StringBuilder result = new StringBuilder();
            char[] chars = s.ToCharArray();
            foreach (int i in chars)
            {
                if (decimaal)
                {
                    result.Append(i.ToString("00 "));
                }
                else
                {
                    result.AppendFormat("{0:x2} ", i);
                }
            }
            return result.ToString();
        }


        public int wordvalue(string s, int offset)
        {
            byte[] chars = System.Text.Encoding.ASCII.GetBytes(s.ToLower());
            int v = 0;
            byte charA = System.Text.Encoding.ASCII.GetBytes("a")[0];
            byte charZ = System.Text.Encoding.ASCII.GetBytes("z")[0];
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= charA && chars[i] <= charZ)
                {
                    v += (chars[i] - charA + offset);
                }
            }
            return v;
        }

        public int wordvalueReverse(string s, int offset)
        {
            byte[] chars = System.Text.Encoding.ASCII.GetBytes(s.ToLower());
            int v = 0;
            byte charA = System.Text.Encoding.ASCII.GetBytes("a")[0];
            byte charZ = System.Text.Encoding.ASCII.GetBytes("z")[0];
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= charA && chars[i] <= charZ)
                {
                    v += (charZ - chars[i] + offset);
                }
            }
            return v;
        }



        public int wordvalueCharacterCount(string s, bool inclSpace)
        {
            char[] chars = s.ToLower().ToCharArray();
            int v = 0;
            string allChars;
            if (inclSpace)
            {
                allChars = "abcdefghijklmnopqrstuvwxyz ";
            }
            else
            {
                allChars = "abcdefghijklmnopqrstuvwxyz";
            }
            for (int i = 0; i < chars.Length; i++)
            {
                if (allChars.IndexOf(chars[i]) >= 0)
                {
                    v++;
                }
            }
            return v;
        }


    }
}
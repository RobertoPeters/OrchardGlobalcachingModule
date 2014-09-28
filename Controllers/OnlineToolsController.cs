using Gavaghan.Geodesy;
using Globalcaching.Core;
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

        public ActionResult FrequencyCounter(string id)
        {
            StringBuilder sb = new StringBuilder();
            int[] val = frequent(id);
            byte charA = System.Text.Encoding.ASCII.GetBytes("a")[0];
            byte[] ch = new byte[1];
            for (byte i = 0; i < 26; i++)
            {
                if (val[i] > 0)
                {
                    ch[0] = (byte)(charA + i);
                    sb.AppendFormat("{0}={1}", System.Text.Encoding.ASCII.GetChars(ch)[0], val[i]);
                    sb.AppendLine();
                }
            }
            return Content(sb.ToString());
        }

        public ActionResult Cypher(string id)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < 26; i++)
            {
                sb.AppendFormat("=== verschuiving van {0} letter ", i);
                sb.AppendLine();
                sb.AppendLine(CipherWithOffset(id, i));
            }
            return Content(sb.ToString());
        }

        public ActionResult Projection(string coord, int distance, int angle)
        {
            string result = "";
            try
            {
                LatLon ll = LatLon.FromString(coord);
                GeodeticCalculator gc = new GeodeticCalculator();
                GlobalCoordinates p = gc.CalculateEndingGlobalCoordinates(Ellipsoid.WGS84, new GlobalCoordinates(new Angle(ll.lat), new Angle(ll.lon)), new Angle(angle), distance);
                result = Helper.GetCoordinatesPresentation(p.Latitude.Degrees, p.Longitude.Degrees);
            }
            catch
            {
                result = "FOUT";
            }
            return Content(result);
        }

        public ActionResult CoordConv(string id)
        {
            string result = "";
            try
            {
                if (id.IndexOfAny(new char[] { 'N', 'n' }) >= 0)
                {
                    LatLon ll = LatLon.FromString(id);
                    if (ll != null)
                    {
                        double x;
                        double y;
                        if (LatLon.RDFromLatLong(ll.lat, ll.lon, out x, out y))
                        {
                            result = string.Format("{0:0} {1:0}", x, y);
                        }
                    }
                }
                else
                {
                    string[] parts = id.Split(new char[] { ' ', ',', 'x', 'y' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        LatLon ll = LatLon.FromRD(Helper.ConvertToDouble(parts[0]), Helper.ConvertToDouble(parts[1]));
                        if (ll != null)
                        {
                            result = Helper.GetCoordinatesPresentation(ll.lat, ll.lon);
                        }
                    }
                }
            }
            catch
            {
            }
            if (result.Length == 0)
            {
                result = "Error";
            }
            return Content(result);
        }

        public ActionResult AfstandHoek(string coord1, string coord2)
        {
            string[] result = null;
            try
            {
                LatLon l1 = LatLon.FromString(coord1);
                LatLon l2 = LatLon.FromString(coord2);
                GeodeticMeasurement gm = Helper.CalculateDistance(l1, l2);
                result = new string[] { gm.EllipsoidalDistance.ToString("0"), gm.Azimuth.Degrees.ToString("0") };
            }
            catch
            {
                result = new string[] { "FOUT", "FOUT" };
            }
            return Json(result);
        }

        public string CipherWithOffset(string s, int offset)
        {
            byte[] chars = System.Text.Encoding.ASCII.GetBytes(s.ToLower());
            byte charA = System.Text.Encoding.ASCII.GetBytes("a")[0];
            byte charZ = System.Text.Encoding.ASCII.GetBytes("z")[0];
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= charA && chars[i] <= charZ)
                {
                    chars[i] += (byte)offset;
                    if (chars[i] > charZ)
                    {
                        chars[i] -= 26;
                    }
                }
            }
            return System.Text.Encoding.ASCII.GetString(chars);
        }

        public int[] frequent(string s)
        {
            byte[] chars = System.Text.Encoding.ASCII.GetBytes(s.ToLower());
            byte charA = System.Text.Encoding.ASCII.GetBytes("a")[0];
            byte charZ = System.Text.Encoding.ASCII.GetBytes("z")[0];
            int[] frequent = new int[26];
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= charA && chars[i] <= charZ)
                {
                    frequent[chars[i] - charA]++;
                }
            }
            return frequent;
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
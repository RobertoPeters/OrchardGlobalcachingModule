using Gavaghan.Geodesy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace Globalcaching.Core
{
    public class Helper
    {
        public static string GetTableNameFromConnectionString(string conStr)
        {
            //"Data Source=globalcaching.eu;Initial Catalog=Globalcaching;Persist Security Info=True;User ID=????;Password=????;Connect Timeout=45;"
            string result = null;
            int pos1 = conStr.IndexOf("Catalog");
            pos1 = conStr.IndexOf("=", pos1);
            int pos2 = conStr.IndexOf(";", pos1);
            result = conStr.Substring(pos1 + 1, pos2 - pos1-1);
            return result;
        }

        public static int GetCacheIDFromCacheCode(string cacheCode)
        {
            const string v = "0123456789ABCDEFGHJKMNPQRTVWXYZ";

            int result = 0;
            try
            {
                string s = cacheCode.Substring(2).ToUpper();
                int baseValue = 31;
                if (s.Length < 4 || (s.Length == 4 && s[0] <= 'F'))
                {
                    baseValue = 16;
                }
                int mult = 1;
                while (s.Length > 0)
                {
                    char c = s[s.Length - 1];
                    result += mult * v.IndexOf(c);
                    mult *= baseValue;
                    s = s.Substring(0, s.Length - 1);
                }
                if (baseValue > 16)
                {
                    result -= 411120;
                }
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        public static string GetCacheCodeFromCacheID(int cacheId)
        {
            const string v = "0123456789ABCDEFGHJKMNPQRTVWXYZ";

            string result = "";
            try
            {
                int i = cacheId;
                int baseValue = 31;
                if (i <= 65535)
                {
                    baseValue = 16;
                }
                else
                {
                    i += 411120;
                }
                while (i > 0)
                {
                    result = string.Concat(v[i % baseValue], result);
                    i /= baseValue;
                }
                result = string.Concat("GC", result);
            }
            catch
            {
                result = "";
            }
            return result;
        }

        public static string GetCityName(LatLon ll)
        {
            string result = "";
            try
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false", ll.lat.ToString().Replace(',', '.'), ll.lon.ToString().Replace(',', '.')));
                wr.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                wr.Method = WebRequestMethods.Http.Get;
                HttpWebResponse webResponse = (HttpWebResponse)wr.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string doc = reader.ReadToEnd();
                webResponse.Close();
                if (doc != null && doc.Length > 0)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(doc);
                    XmlNodeList nl = xdoc.SelectNodes("GeocodeResponse/result/address_component");
                    foreach (XmlNode n in nl)
                    {
                        XmlNode nt = n.SelectSingleNode("type");
                        if (nt != null && nt.InnerText == "locality")
                        {
                            result = n.SelectSingleNode("long_name").InnerText;
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public static LatLon GetLocation(string loc)
        {
            LatLon result = LatLon.FromString(loc);
            if (result == null)
            {
                result = GetLocationOfCity(loc);
            }
            return result;
        }

        public static LatLon GetLocationOfCity(string city)
        {
            LatLon result = null;
            try
            {
                string s = null;
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", HttpUtility.UrlEncode(city)));
                wr.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                wr.Method = WebRequestMethods.Http.Get;
                using (HttpWebResponse webResponse = (HttpWebResponse)wr.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        s = reader.ReadToEnd();
                        webResponse.Close();
                    }
                }
                if (s != null && s.Length > 0)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(s);
                    XmlNode n = xdoc.SelectSingleNode("GeocodeResponse/result/geometry/location");
                    result = new LatLon();
                    result.lat = ConvertToDouble(n.SelectSingleNode("lat").InnerText);
                    result.lon = ConvertToDouble(n.SelectSingleNode("lng").InnerText);
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public static string GetCoordinatesPresentation(double lat, double lon)
        {
            double minutes1 = 60.0 * (lat - (int)lat);
            double minutes2 = 60.0 * (lon - (int)lon);
            string d1 = (1000.0 * (minutes1 - (int)minutes1)).ToString("000");
            if (d1.Length > 3)
            {
                minutes1 += 1;
                d1 = "000";
            }
            string d2 = (1000.0 * (minutes2 - (int)minutes2)).ToString("000");
            if (d2.Length > 3)
            {
                minutes2 += 1;
                d2 = "000";
            }
            return string.Concat(
                "N ", ((int)(lat)).ToString(), "° ", ((int)minutes1).ToString("00"), ".", d1, " ",
                "E ", ((int)(lon)).ToString(), "° ", ((int)minutes2).ToString("00"), ".", d2
                );
        }

        public static double ConvertToDouble(string s)
        {
            return Convert.ToDouble(s.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        public static GeodeticMeasurement CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            GlobalCoordinates p1 = new GlobalCoordinates(new Angle(lat1), new Angle(lon1));
            GlobalCoordinates p2 = new GlobalCoordinates(new Angle(lat2), new Angle(lon2));
            GeodeticCalculator gc = new GeodeticCalculator();
            GlobalPosition gp1 = new GlobalPosition(p1);
            GlobalPosition gp2 = new GlobalPosition(p2);
            GeodeticMeasurement gm = gc.CalculateGeodeticMeasurement(Ellipsoid.WGS84, gp1, gp2);
            return gm;
        }

        public static string GetWindDirection(Angle a)
        {
            string result = "";
            double deg = a.Degrees;
            if (deg < 0)
            {
                deg += 360;
            }
            if (deg >= (360 - 22.5) || deg <= 22.5) result = "N";
            else if (deg >= (45 - 22.5) && deg <= (45 + 22.5)) result = "NE";
            else if (deg >= (90 - 22.5) && deg <= (90 + 22.5)) result = "E";
            else if (deg >= (135 - 22.5) && deg <= (135 + 22.5)) result = "SE";
            else if (deg >= (180 - 22.5) && deg <= (180 + 22.5)) result = "S";
            else if (deg >= (225 - 22.5) && deg <= (225 + 22.5)) result = "SW";
            else if (deg >= (270 - 22.5) && deg <= (270 + 22.5)) result = "W";
            else if (deg >= (315 - 22.5) && deg <= (315 + 22.5)) result = "NW";
            return result;
        }

        public static GeodeticMeasurement CalculateDistance(LatLon l1, LatLon l2)
        {
            GlobalCoordinates p1 = new GlobalCoordinates(new Angle(l1.lat), new Angle(l1.lon));
            GlobalCoordinates p2 = new GlobalCoordinates(new Angle(l2.lat), new Angle(l2.lon));
            GeodeticCalculator gc = new GeodeticCalculator();
            GlobalPosition gp1 = new GlobalPosition(p1);
            GlobalPosition gp2 = new GlobalPosition(p2);
            GeodeticMeasurement gm = gc.CalculateGeodeticMeasurement(Ellipsoid.WGS84, gp1, gp2);
            return gm;
        }

        public static string DecryptHint(string s)
        {
            string result;
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                //we might do a typecase and calculate
                //but lets not think now
                if (chars[i] == 'a')
                    chars[i] = 'n';
                else if (chars[i] == 'A')
                    chars[i] = 'N';
                else if (chars[i] == 'b')
                    chars[i] = 'o';
                else if (chars[i] == 'B')
                    chars[i] = 'O';
                else if (chars[i] == 'c')
                    chars[i] = 'p';
                else if (chars[i] == 'C')
                    chars[i] = 'P';
                else if (chars[i] == 'd')
                    chars[i] = 'q';
                else if (chars[i] == 'D')
                    chars[i] = 'Q';
                else if (chars[i] == 'e')
                    chars[i] = 'r';
                else if (chars[i] == 'E')
                    chars[i] = 'R';
                else if (chars[i] == 'f')
                    chars[i] = 's';
                else if (chars[i] == 'F')
                    chars[i] = 'S';
                else if (chars[i] == 'g')
                    chars[i] = 't';
                else if (chars[i] == 'G')
                    chars[i] = 'T';
                else if (chars[i] == 'h')
                    chars[i] = 'u';
                else if (chars[i] == 'H')
                    chars[i] = 'U';
                else if (chars[i] == 'i')
                    chars[i] = 'v';
                else if (chars[i] == 'I')
                    chars[i] = 'V';
                else if (chars[i] == 'j')
                    chars[i] = 'w';
                else if (chars[i] == 'J')
                    chars[i] = 'W';
                else if (chars[i] == 'k')
                    chars[i] = 'x';
                else if (chars[i] == 'K')
                    chars[i] = 'X';
                else if (chars[i] == 'l')
                    chars[i] = 'y';
                else if (chars[i] == 'L')
                    chars[i] = 'Y';
                else if (chars[i] == 'm')
                    chars[i] = 'z';
                else if (chars[i] == 'M')
                    chars[i] = 'Z';
                else if (chars[i] == 'n')
                    chars[i] = 'a';
                else if (chars[i] == 'N')
                    chars[i] = 'A';
                else if (chars[i] == 'o')
                    chars[i] = 'b';
                else if (chars[i] == 'O')
                    chars[i] = 'b';
                else if (chars[i] == 'p')
                    chars[i] = 'c';
                else if (chars[i] == 'P')
                    chars[i] = 'C';
                else if (chars[i] == 'q')
                    chars[i] = 'd';
                else if (chars[i] == 'Q')
                    chars[i] = 'D';
                else if (chars[i] == 'r')
                    chars[i] = 'e';
                else if (chars[i] == 'R')
                    chars[i] = 'E';
                else if (chars[i] == 's')
                    chars[i] = 'f';
                else if (chars[i] == 'S')
                    chars[i] = 'F';
                else if (chars[i] == 't')
                    chars[i] = 'g';
                else if (chars[i] == 'T')
                    chars[i] = 'G';
                else if (chars[i] == 'u')
                    chars[i] = 'h';
                else if (chars[i] == 'U')
                    chars[i] = 'H';
                else if (chars[i] == 'v')
                    chars[i] = 'i';
                else if (chars[i] == 'V')
                    chars[i] = 'I';
                else if (chars[i] == 'w')
                    chars[i] = 'j';
                else if (chars[i] == 'W')
                    chars[i] = 'J';
                else if (chars[i] == 'x')
                    chars[i] = 'k';
                else if (chars[i] == 'X')
                    chars[i] = 'K';
                else if (chars[i] == 'y')
                    chars[i] = 'l';
                else if (chars[i] == 'Y')
                    chars[i] = 'L';
                else if (chars[i] == 'z')
                    chars[i] = 'm';
                else if (chars[i] == 'Z')
                    chars[i] = 'M';
            }
            result = new string(chars);
            return result;
        }

    }
}
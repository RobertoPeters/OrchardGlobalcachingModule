using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class Athom : Controller
    {
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult webhooks(string id, string token)
        {
            string ev = Request.QueryString["event"];
            string data1 = Request.QueryString["data1"] ?? "";
            string data2 = Request.QueryString["data2"] ?? "";
            string data3 = Request.QueryString["data3"] ?? "";

            var url = string.Format("https://webhooks.athom.com/webhook/{0}/?token={1}", id, token);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.17 Safari/533.4";
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = string.Format("{{ \"event\": \"{0}\"}}", ev);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            return null;
        }
    }
}
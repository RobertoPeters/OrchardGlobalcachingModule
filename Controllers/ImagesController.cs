using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    [ValidateInput(false)]
    public class ImagesController: Controller
    {
        public ImagesController()
        {
        }

        public ActionResult GeocachingComCacheLargeImage(string code, string id)
        {
            //<img src="http://imgcdn.geocaching.com/cache/large/a3b2712b-a9f5-43b0-88ef-05cc18c076f4.jpg" class="InsideTable" style="max-width: 670px;">
            //cache/large/GC5J1FQ/a3b2712b-a9f5-43b0-88ef-05cc18c076f4.jpg

            byte[] data = null;
            string fn = System.IO.Path.Combine(GetFolder(code, false), id);
            if (System.IO.File.Exists(fn))
            {
                data = System.IO.File.ReadAllBytes(fn);
            }
            else
            {
                using (var wc = new WebClient())
                {
                    try
                    {
                        data = wc.DownloadData(string.Format("http://imgcdn.geocaching.com/cache/large/{0}", id));
                    }
                    catch
                    {
                    }
                    if (data != null)
                    {
                        GetFolder(code, true);
                        System.IO.File.WriteAllBytes(fn, data);
                    }
                }
            }

            if (data != null)
            {
                try
                {
                    Response.ContentType = string.Format("image/{0}", System.IO.Path.GetExtension(id).Replace(".", "").ToLower());
                    Response.OutputStream.Write(data, 0, data.Length);
                }
                catch
                {
                }
            }
            return null;
        }

        private string GetFolder(string gcCode, bool create)
        {
            //string result = HttpContext.Server.MapPath("/Modules/Globalcaching/GeocacheImages");
            string result = "c:\\GeocacheImages";
            if (create && !System.IO.Directory.Exists(result))
            {
                System.IO.Directory.CreateDirectory(result);
            }
            result = System.IO.Path.Combine(result, gcCode[gcCode.Length - 1].ToString());
            if (create && !System.IO.Directory.Exists(result))
            {
                System.IO.Directory.CreateDirectory(result);
            }
            result = System.IO.Path.Combine(result, gcCode[gcCode.Length - 2].ToString());
            if (create && !System.IO.Directory.Exists(result))
            {
                System.IO.Directory.CreateDirectory(result);
            }
            result = System.IO.Path.Combine(result, gcCode);
            if (create && !System.IO.Directory.Exists(result))
            {
                System.IO.Directory.CreateDirectory(result);
            }
            return result;
        }
    }
}
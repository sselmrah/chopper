using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;
//учеба
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace chopper1.Controllers
{
    public class HomeController : Controller
    {
        //Учебная часть
        private static BlockingCollection<string> _data = new BlockingCollection<string>();
        static HomeController()
        {
            _data.Add("started");
            for (int i = 0; i < 10; i++)
            {
                _data.Add("item" + i.ToString());
            }
            _data.Add("ended");
        }
        //Конец учебной части
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        public ActionResult Message()
        {
        //Учебно-боевой модуль SSE    
            var result = string.Empty;
            var sb = new StringBuilder();
            if (_data.TryTake(out result, TimeSpan.FromMilliseconds(1000)))
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var serializedObject = ser.Serialize(new { item = result, message = "hello" });
                sb.AppendFormat("data: {0}\n\n", serializedObject);
            }
            var x = sb.ToString();

            var y = "data: Hey\n\n";
            return Content(y, "text/event-stream");

            //return Content(sb.ToString(), "text/event-stream");
        }

    }
}
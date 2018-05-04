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
        private static Models.ApplicationDbContext db = chopper1.Startup.context;
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


        public ActionResult Settings()
        {
            var set = Startup.context.Settings.Where(s => s.UserName == "Global").FirstOrDefault();
            return View(set);
        }

        [HttpPost]
        public ActionResult UpdateColor(string color, string value)
        {
            string hexColor = value;
            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(value);
            var set = db.Settings.Where(s => s.UserName == "Global").FirstOrDefault();
            string rgbColor = c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString();
            switch (color)
            {
                case "red1":                    
                    set.Red1 = rgbColor;
                    break;
                case "red2":
                    set.Red2 = rgbColor;
                    break;
                case "red3":
                    set.Red3 = rgbColor;
                    break;
                case "green1":
                    set.Green1 = rgbColor;
                    break;
                case "green2":
                    set.Green2 = rgbColor;
                    break;
                case "green3":
                    set.Green3 = rgbColor;
                    break;
            }
            db.SaveChanges();            
            
            return View("Settings",set);
        }

        [HttpGet]
        public string GetReds()
        {
            var set = db.Settings.Where(s => s.UserName == "Global").FirstOrDefault();
            string[] r1 = set.Red1.Split(',');
            System.Drawing.Color r1c = System.Drawing.Color.FromArgb(255, Convert.ToInt32(r1[0]), Convert.ToInt32(r1[1]), Convert.ToInt32(r1[2]));
            string[] r2 = set.Red2.Split(',');
            System.Drawing.Color r2c = System.Drawing.Color.FromArgb(255, Convert.ToInt32(r2[0]), Convert.ToInt32(r2[1]), Convert.ToInt32(r2[2]));
            string[] r3 = set.Red3.Split(',');
            System.Drawing.Color r3c = System.Drawing.Color.FromArgb(255, Convert.ToInt32(r3[0]), Convert.ToInt32(r3[1]), Convert.ToInt32(r3[2]));
            string[] g1 = set.Green1.Split(',');
            System.Drawing.Color g1c = System.Drawing.Color.FromArgb(255, Convert.ToInt32(g1[0]), Convert.ToInt32(g1[1]), Convert.ToInt32(g1[2]));
            string[] g2 = set.Green2.Split(',');
            System.Drawing.Color g2c = System.Drawing.Color.FromArgb(255, Convert.ToInt32(g2[0]), Convert.ToInt32(g2[1]), Convert.ToInt32(g2[2]));
            string[] g3 = set.Green3.Split(',');
            System.Drawing.Color g3c = System.Drawing.Color.FromArgb(255, Convert.ToInt32(g3[0]), Convert.ToInt32(g3[1]), Convert.ToInt32(g3[2]));
            string result = "#"+ r1c.R.ToString("X2") + r1c.G.ToString("X2") + r1c.B.ToString("X2")+ ","+
                            "#" + r2c.R.ToString("X2") + r2c.G.ToString("X2") + r2c.B.ToString("X2") + ","+
                            "#" + r3c.R.ToString("X2") + r3c.G.ToString("X2") + r3c.B.ToString("X2") + "," +
                            "#" + g1c.R.ToString("X2") + g1c.G.ToString("X2") + g1c.B.ToString("X2") + "," +
                            "#" + g2c.R.ToString("X2") + g2c.G.ToString("X2") + g2c.B.ToString("X2") + "," +
                            "#" + g3c.R.ToString("X2") + g3c.G.ToString("X2") + g3c.B.ToString("X2");

            return result;
        }

        public ActionResult DefaultColor(string color)
        {
            var set = db.Settings.Where(s => s.UserName == "Global").FirstOrDefault();
            var defaultC = db.Settings.Where(s => s.UserName == "Default").FirstOrDefault();
            
            switch (color)
            {
                case "red1":
                    set.Red1 = defaultC.Red1;
                    break;
                case "red2":
                    set.Red2 = defaultC.Red2;
                    break;
                case "red3":
                    set.Red3 = defaultC.Red3;
                    break;
                case "green1":
                    set.Green1 = defaultC.Green1;
                    break;
                case "green2":
                    set.Green2 = defaultC.Green2;
                    break;
                case "green3":
                    set.Green3 = defaultC.Green3;
                    break;
            }
            db.SaveChanges();
            return View("Settings", set);
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
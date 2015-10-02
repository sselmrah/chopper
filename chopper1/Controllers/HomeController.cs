using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;

namespace chopper1.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult SelectCategory()
        {

            List<SelectListItem> weeks = new List<SelectListItem>();
            WebСервис1 curWc = new WebСервис1();
            TVWeekType curTvWeek = new TVWeekType();
            try
            {
                curWc.Credentials = new System.Net.NetworkCredential("mike", "123");
                TVWeekType[] weeks1 = curWc.GetWeeks();
                int i = 0;
                foreach (TVWeekType week in weeks1)
                {
                    i += 1;
                    weeks.Add(new SelectListItem { Text = week.Note, Value = i.ToString() });
                }
          
            }
            catch
            {

            }

            ViewBag.MovieType = weeks;

            return View();

        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;
using chopper1.Models;
using System.Threading.Tasks;
using Omu.ValueInjecter;




namespace chopper1.Controllers
{
    public class WeeksController : Controller
    {
        private WebСервис1 curWc = MyStartupClass.wc;
        // GET: Weeks
        public ActionResult Index()
        {
            List<SelectListItem> weeks = new List<SelectListItem>();
            //WebСервис1 curWc = new WebСервис1();
            TVWeekType curTvWeek = new TVWeekType();
            try
            {
                curWc = chopper1.MyStartupClass.wc;                
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
        public ActionResult constructWeek(Week week)
        {

            return View(week);
        }
        public ActionResult getWeek()
        {
            Week curWeek = new Week();
            //Experiments
            TVWeekType curTvWeek= new TVWeekType();
            try
            {
               // curWc.Credentials = new System.Net.NetworkCredential("mike", "123");
                TVWeekType[] weeks = curWc.GetWeeks();
                curTvWeek= weeks[10];                
                ViewData["weekName"] = curWeek.Name;
                
            }
            catch
            {

            }

            WeekTVDayType[] daysOfWeek = getDaysOfWeek(curTvWeek);
            //EfirType[] efirs = getEfirsByTVday(daysOfWeek[20]);
            curWeek.InjectFrom(curTvWeek);
            curWeek.DaysCount = daysOfWeek.Length;
            curWeek.Days = daysOfWeek;
            ViewData["daysCount"]= daysOfWeek.Length;
            
            return View(curWeek);
        }

        private WeekTVDayType[] getDaysOfWeek(TVWeekType week)
        {
            int[] array_channel_codes = new int[3];
            array_channel_codes[0] = 10;
            array_channel_codes[1] = 21;
            array_channel_codes[2] = 40;

            WeekTVDayType[] weekTVday = curWc.GetWeekTVDays(week.Ref, array_channel_codes);

            return weekTVday;
        }




    }
}
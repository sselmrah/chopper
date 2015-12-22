using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;
using chopper1.Models;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using System.Web.Script.Serialization;
//учеба
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;




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
        public ActionResult getWeek(string week_num="10")
        {
            Week curWeek = new Week();
            //Experiments
            TVWeekType curTvWeek= new TVWeekType();
            try
            {
               // curWc.Credentials = new System.Net.NetworkCredential("mike", "123");
                TVWeekType[] weeks = curWc.GetWeeks();                
                //curTvWeek= weeks[Convert.ToInt32(week_num)];                
                curTvWeek = weeks[MyStartupClass.getWeekInWork(weeks)];
                ViewData["weekName"] = curWeek.Name;
                
            }
            catch
            {

            }


            int[] array_channel_codes = new int[1];
            array_channel_codes[0] = 10;
            //array_channel_codes[1] = 11;
            //array_channel_codes[2] = 12;

            List<WeekTVDayType> daysOfWeek = getDaysOfWeek(curTvWeek, array_channel_codes).OrderBy(o => o.TVDate).ToList();
            //List<WeekTVDayType> SortedList = daysOfWeek.OrderBy(o => o.TVDate).ToList();
            //EfirType[] efirs = getEfirsByTVday(daysOfWeek[20]);
            curWeek.InjectFrom(curTvWeek);
            curWeek.DaysCount = daysOfWeek.Count();            
            curWeek.Days = daysOfWeek;
            ViewData["daysCount"]= daysOfWeek.Count();                        

            return View(curWeek);
        }

        private List<WeekTVDayType> getDaysOfWeek(TVWeekType week, int[] array_channel_codes)
        {
            

            List<WeekTVDayType> weekTVday = curWc.GetWeekTVDays(week.Ref, array_channel_codes).ToList();

            return weekTVday;
        }
        
        public ActionResult SelectCategory()
        {
            
            List<SelectListItem> weeks = new List<SelectListItem>();
            /*
            WebСервис1 curWc = new WebСервис1();
            TVWeekType curTvWeek = new TVWeekType();
            try
            {
                curWc.Credentials = new System.Net.NetworkCredential("mike", "123");
                TVWeekType[] weeks1 = curWc.GetWeeks();
             */ 
                int i = 0;
                foreach (TVWeekType week in chopper1.MyStartupClass.tvWeeks)
                {
                    weeks.Add(new SelectListItem { Value = i.ToString(), Text = week.Note});
                    i += 1;
                }

            /*}
            catch
            {
                
            }*/
            var selectList = new SelectList(weeks, "Value", "Text", chopper1.MyStartupClass.selectedID);
            
            ViewData["Weeks1"] = selectList;
            ViewBag.Week = weeks;
            return View();

        }
        
        public ViewResult WeekChosen(string WeekID)
        {
            chopper1.MyStartupClass.selectedID = Convert.ToInt32(WeekID);
            ViewBag.messageString = WeekID;
            ViewBag.weeknum = WeekID;


            return View("SelectWeek");

        }

        public class MyViewModel
        {
            public int SelectedWeekId { get; set; }
            public SelectList WeeksList { get; set; }

            // Other properties you need in your view
        }



        public ActionResult Stolby(string week_num = "10")
        {            
            Week curWeek = new Week();

            TVWeekType curTvWeek= new TVWeekType();

            curTvWeek = MyStartupClass.tvWeeks[Convert.ToInt32(week_num)];                
            ViewData["weekName"] = curWeek.Name;

            int[] array_channel_codes = new int[5];
            array_channel_codes[0] = 10;
            array_channel_codes[1] = 11;
            array_channel_codes[2] = 12;
            array_channel_codes[3] = 13;
            array_channel_codes[4] = 14;

            List<WeekTVDayType> allDays = getDaysOfWeek(curTvWeek, array_channel_codes).OrderBy(o => o.TVDate).ToList();
            List<WeekTVDayType> chOneDays = new List<WeekTVDayType>();

            foreach (WeekTVDayType curDay in allDays)
            {
                if (curDay.KanalKod==10)
                {
                    chOneDays.Add(curDay);
                }
            }
            //Добавляем значения свойств в новый объект из старого
            curWeek.InjectFrom(curTvWeek);
            
            //Собираем дни недели для ПК и орбит
            List<WeekTVDayType> daysToAdd = new List<WeekTVDayType>();
            foreach(WeekTVDayType wday in chOneDays)
            {
                 daysToAdd.AddRange(getOrbitsByChannelOneDay(wday, allDays));                
            }
            curWeek.Days = daysToAdd;

            //Складываем в список дней для проверки
            //foreach (WeekTVDayType weekDay in curWeek.Days)
            //{             
            //    chopper1.MyStartupClass.days_to_check.Add(weekDay);
            //}
            curWeek.DaysCount = daysToAdd.Count() / 5; //Т.к. каждый день - это ПК+4 орбиты
            return View(curWeek);
        
        }

        [HttpGet]
        //public ActionResult Broadcast(string dateStr="2015-12-17")
        public ActionResult Broadcast()
        {
            string dateStr = "";

            if (Request["bdate"] == null)
            {
                dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");
            }       
            else
            {                
                dateStr = Request["bdate"];
                if (dateStr.Length==0)
                {
                    dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");
                }
            }


            Week curWeek = new Week();
            TVWeekType curTvWeek = new TVWeekType();
            DateTime curDate = DateTime.Parse(dateStr);
            List<WeekTVDayType> days = new List<WeekTVDayType>();
            WeekTVDayType chOneDay = new WeekTVDayType();

            int[] array_channel_codes = new int[5];
            array_channel_codes[0] = 10;
            array_channel_codes[1] = 11;
            array_channel_codes[2] = 12;
            array_channel_codes[3] = 13;
            array_channel_codes[4] = 14;

            foreach(TVWeekType TvWeek in MyStartupClass.tvWeeks)
            {
                if (curDate >= TvWeek.BegDate & (curDate - TvWeek.BegDate).Days<7)
                {
                    curTvWeek = TvWeek;
                    WeekTVDayType[] weekDays = curWc.GetWeekTVDays(curTvWeek.Ref, array_channel_codes);
                    foreach (WeekTVDayType day in weekDays)
                    {
                        if (day.TVDate == curDate)
                        {
                            if (day.KanalKod == 10) chOneDay = day;
                            days.Add(day);                          
                        }
                    }
                    //Хотя нужно бы по-надежнее все сделать.
                    break;
                }
            }

            //Добавляем значения свойств в новый объект из старого
            curWeek.InjectFrom(curTvWeek);

            //Собираем дни недели для ПК и орбит
            List<WeekTVDayType> daysToAdd = new List<WeekTVDayType>();            
            daysToAdd.AddRange(getOrbitsByChannelOneDay(chOneDay, days, true));            
            curWeek.Days = daysToAdd;

            //Складываем в список дней для проверки
            //foreach (WeekTVDayType weekDay in curWeek.Days)
            //{             
            //    chopper1.MyStartupClass.days_to_check.Add(weekDay);
            //}
            curWeek.DaysCount = daysToAdd.Count() / 5; //Т.к. каждый день - это ПК+4 орбиты
            return View(curWeek);

        }


        private List<WeekTVDayType> getOrbitsByChannelOneDay(WeekTVDayType chOneDay, List<WeekTVDayType> allDays, bool reverse = false)
        {
            List<WeekTVDayType> orbits = new List<WeekTVDayType>();            
            //Добавили Первый канал
            orbits.Add(chOneDay);
            //Начали добавлять орбиты
            for (int chCode = 11; chCode <= 14; chCode++)
            {
                TVDayVariantType[] variants = curWc.GetDayVariants(chOneDay.TVDate, chCode);
                //Если нет вариантов для орбиты - добавляем null
                if (variants.Count() == 0)
                {                    
                    //orbits.Add(null);
                    orbits.Add(new WeekTVDayType());
                }
                else
                {
                    //Если для данной орбиты есть только один вариант, то добавляем его
                    if (variants.Count() == 1)
                    {
                        foreach (WeekTVDayType day in allDays)
                        {
                            if (chOneDay.TVDate == day.TVDate & day.KanalKod == chCode)
                            {
                                if (!reverse)
                                {
                                    orbits.Add(day);
                                }
                                else
                                {
                                    orbits.Insert(1, day);
                                }
                                break;
                            }
                        }
                    }
                    //Если для данной орбиты >1 варианты, выбираем совпадающий по номеру с ПК
                    else
                    {
                        foreach (WeekTVDayType day in allDays)
                        {
                            if (chOneDay.TVDate == day.TVDate & day.KanalKod == chCode & day.VariantKod == day.VariantKod)
                            {
                                if (!reverse)
                                {
                                    orbits.Add(day);
                                }
                                else
                                {
                                    orbits.Insert(1, day);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            



            return orbits;
        }

        public ActionResult CheckDays()
        {
            //List<TVDayVariantT> variants = new List<TVDayVariantT>();
            TVDayVariantT curVariant = new TVDayVariantT();
            DateTime curDate = DateTime.Now + TimeSpan.FromDays(1);
            foreach(Day day in chopper1.MyStartupClass.days_to_check)
            {
                //curVariant.TVDayRef = day.TVDayRef;
                //curVariant.VariantNumber = day.VariantKod;
                //variants.Add(curVariant);
                if (curDate>day.RenderTime)
                {
                    curDate = day.RenderTime;
                }
            }

            //TVDayVariantT[] t = variants.ToArray();            
            string dayToUpdateRef = "";
            
            TVDayVariantT[] rez = curWc.CheckVariants(chopper1.MyStartupClass.variants_to_check.ToArray(), curDate);
            //DateTime curTime = curWc.GetCurrentTime();
            //int testCount = rez.Count();

            if (rez.Count() > 0)
            {
                if (chopper1.MyStartupClass.variants_to_update.Count() == 0)
                {
                    chopper1.MyStartupClass.variants_to_update = rez.ToList();
                }
                else
                {
                    dayToUpdateRef = chopper1.MyStartupClass.variants_to_update[0].TVDayRef;
                    chopper1.MyStartupClass.variants_to_update.RemoveAt(0);
                }
            }
            
            var y = "data: "+dayToUpdateRef+"\n\n";            
            return Content(y, "text/event-stream");
        }

        [HttpPost]
        public ActionResult UpdateDayStolby()
        {
            string curDayRef= Request["HTTP_DAYID"];
            Day curDay = new Day();          
                        
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef)
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    curDay = d;
                    break;
                }
            }

            curDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            return View(curDay);
        }

        [HttpPost]
        public ActionResult UpdateDayWeek()
        {
            string curDayRef = Request["HTTP_DAYID"];
            Day curDay = new Day();

            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef)
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    curDay = d;
                    break;
                }
            }

            curDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            return View(curDay);
        }

        [HttpPost]
        public ActionResult UpdateDayBroadcast()
        {
            string curDayRef = Request["HTTP_DAYID"];
            Day curDay = new Day();

            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef)
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    curDay = d;
                    break;
                }
            }

            curDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            return View(curDay);
        }

    }
}
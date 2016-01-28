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
        public ActionResult getWeek(string week_num="")
        {
            //Чистим список проверяемых дней
            chopper1.MyStartupClass.variants_to_check.Clear();

            Week curWeek = new Week();
            //Experiments
            TVWeekType curTvWeek= new TVWeekType();
            try
            {
                
               // curWc.Credentials = new System.Net.NetworkCredential("mike", "123");
                TVWeekType[] weeks = curWc.GetWeeks();
                //If week_num is not specified get the week currently in work                
                if (week_num.Left(3) == "cur")
                {
                    int shift = 0;                    
                    //Current week = cur0
                    //Current week+1 = cur1
                    //Current week+2 = cur2
                    shift = Convert.ToInt32(week_num.Right(1));
                    
                    curTvWeek = weeks[MyStartupClass.getCurrentWeek(weeks)-shift];
                    MyStartupClass.selectedID = MyStartupClass.getCurrentWeek(weeks)-shift;
                }
                else
                {
                    if (week_num == "")
                    {
                        curTvWeek = weeks[MyStartupClass.getWeekInWork(weeks)];
                        MyStartupClass.selectedID = MyStartupClass.getWeekInWork(weeks);
                    }
                    else
                    {
                        curTvWeek = weeks[weeks.Length - 1 - Convert.ToInt32(week_num)];
                    }
                }
 
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
        
        public ActionResult SelectCategory(string curWeekRef = "", string repType = "raskladka")
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
                int curId = -1;
                foreach (TVWeekType week in chopper1.MyStartupClass.tvWeeks)
                {
                    weeks.Add(new SelectListItem { Value = (chopper1.MyStartupClass.tvWeeks.Length-1-i).ToString(), Text = week.Note});
                    if (week.Ref == curWeekRef)
                    {
                        curId = i;
                    }
                    i += 1;
                }

            /*}
            catch
            {
                
            }*/

            var selectList = new SelectList(weeks, "Value", "Text", MyStartupClass.tvWeeks.Length-1- chopper1.MyStartupClass.selectedID);
            //var selectList = new SelectList(weeks, "Value", "Text", curId);
            
            ViewData["Weeks1"] = selectList;
            ViewData["ReportType"] = repType;
            ViewBag.Week = weeks;
            return View();

        }

        public ViewResult WeekChosen(string WeekID, string repType)
        {
            //int WeekIDint = Convert.ToInt32(WeekID);
            //WeekIDint = chopper1.MyStartupClass.tvWeeks.Length - 1 - WeekIDint;
            //WeekID = WeekIDint.ToString();
            if (WeekID == "")
            {
                WeekID = chopper1.MyStartupClass.getWeekInWork(chopper1.MyStartupClass.tvWeeks).ToString();
            }
            chopper1.MyStartupClass.selectedID = chopper1.MyStartupClass.tvWeeks.Length-1-Convert.ToInt32(WeekID);
            ViewBag.messageString = WeekID;
            ViewBag.weeknum = WeekID;
            ViewBag.reportType = repType;

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
            //Чистим список проверяемых дней
            chopper1.MyStartupClass.days_to_check.Clear();
            chopper1.MyStartupClass.variants_to_check.Clear();

            Week curWeek = new Week();

            TVWeekType curTvWeek= new TVWeekType();

            //***Adding week selector from index page
            try
            {             
                TVWeekType[] weeks = curWc.GetWeeks();
                //If week_num is not specified get the week currently in work                
                if (week_num.Left(3) == "cur")
                {
                    int shift = 0;
                    //Current week = cur0
                    //Current week+1 = cur1
                    //Current week+2 = cur2
                    shift = Convert.ToInt32(week_num.Right(1));

                    curTvWeek = weeks[MyStartupClass.getCurrentWeek(weeks) - shift];
                    MyStartupClass.selectedID = MyStartupClass.getCurrentWeek(weeks) - shift;
                }
                else
                {
                    if (week_num == "")
                    {
                        curTvWeek = weeks[MyStartupClass.getWeekInWork(weeks)];
                        MyStartupClass.selectedID = MyStartupClass.getWeekInWork(weeks);
                    }
                    else
                    {
                        curTvWeek = weeks[weeks.Length - 1 - Convert.ToInt32(week_num)];
                    }
                }

                ViewData["weekName"] = curWeek.Name;

            }
            catch
            {

            }



            
            //Previous version
            //curTvWeek = MyStartupClass.tvWeeks[Convert.ToInt32(week_num)];                
            //ViewData["weekName"] = curWeek.Name;

            //***

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
            //Чистим список проверяемых дней
            chopper1.MyStartupClass.variants_to_check.Clear();

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

        [HttpGet]
        public ActionResult OrbWeek(string week_num = "", int part_num = 1)
        {
            //Чистим список проверяемых дней
            chopper1.MyStartupClass.variants_to_check.Clear();

            Week curWeek = new Week();
            //Experiments
            TVWeekType curTvWeek = new TVWeekType();
            try
            {

                // curWc.Credentials = new System.Net.NetworkCredential("mike", "123");
                TVWeekType[] weeks = curWc.GetWeeks();
                //If week_num is not specified get the week currently in work                
                if (week_num.Left(3) == "cur")
                {
                    int shift = 0;
                    //Current week = cur0
                    //Current week+1 = cur1
                    //Current week+2 = cur2
                    shift = Convert.ToInt32(week_num.Right(1));

                    curTvWeek = weeks[MyStartupClass.getCurrentWeek(weeks) - shift];
                    MyStartupClass.selectedID = MyStartupClass.getCurrentWeek(weeks) - shift;
                }
                else
                {
                    if (week_num == "")
                    {
                        //curTvWeek = weeks[MyStartupClass.getWeekInWork(weeks)];
                        //MyStartupClass.selectedID = MyStartupClass.getWeekInWork(weeks);
                        curTvWeek = weeks[MyStartupClass.getCurrentWeek(weeks)];
                        MyStartupClass.selectedID = MyStartupClass.getCurrentWeek(weeks);
                    }
                    else
                    {
                        curTvWeek = weeks[weeks.Length - 1 - Convert.ToInt32(week_num)];
                    }
                }

                ViewData["weekName"] = curWeek.Name;

            }
            catch
            {

            }


            List<WeekTVDayType> days = new List<WeekTVDayType>();
            WeekTVDayType chOneDay = new WeekTVDayType();

            int[] array_channel_codes = new int[5];
            array_channel_codes[0] = 10;
            array_channel_codes[1] = 11;
            array_channel_codes[2] = 12;
            array_channel_codes[3] = 13;
            array_channel_codes[4] = 14;


            List<WeekTVDayType> daysOfWeek = getDaysOfWeek(curTvWeek, array_channel_codes).OrderBy(o => o.TVDate).ToList();            
            if (part_num == 1)
            {
                daysOfWeek = daysOfWeek.Take(20).ToList();
            }
            else
            {

            }

            curWeek.InjectFrom(curTvWeek);
            curWeek.DaysCount = daysOfWeek.Count()/5;            
            curWeek.Days = daysOfWeek;
            ViewData["daysCount"]= daysOfWeek.Count();                                                           
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
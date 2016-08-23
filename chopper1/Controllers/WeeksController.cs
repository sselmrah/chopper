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
/*using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
 * */
using Microsoft.AspNet.SignalR;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.IO;


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


        public ActionResult endlessWeek(string week_num = "")
        {
            Week curWeek = new Week();
            TVWeekType curTvWeek = new TVWeekType();
            
            string curTvWeekNum = getWeekNum(week_num);
            curTvWeek = MyStartupClass.tvWeeks[MyStartupClass.tvWeeks.Length - 1 - Convert.ToInt32(curTvWeekNum)];
            ViewBag.WeekId = curTvWeekNum;

            int[] array_channel_codes = new int[1];
            array_channel_codes[0] = 10;
            
            List<WeekTVDayType> daysOfWeek = getDaysOfWeek(curTvWeek, array_channel_codes).OrderBy(o => o.TVDate).ToList();            

            curWeek.InjectFrom(curTvWeek);
            curWeek.DaysCount = daysOfWeek.Count();
            curWeek.Days = daysOfWeek;
            ViewData["daysCount"] = daysOfWeek.Count();

            return View(curWeek);
        }

        public ActionResult alldays(string dayStart="")
        {
            DateTime startDt = DateTime.Now.Date;
            if (dayStart != "")
            { 
                startDt = DateTime.Parse(dayStart); 
            }


            List<Day> daysList = new List<Day>();
            for (int i =-3; i<15;i++)
            {
                Day curDay = new Day();
                try
                {
                    curDay = MyStartupClass.getDayByDateAndVariantCode(startDt + TimeSpan.FromDays(i), 1);
                }
                catch
                {
                    curDay.TVDate = startDt + TimeSpan.FromDays(i);
                }
                daysList.Add(curDay);
            }           

            return View(daysList);
        }



        public ActionResult getWeek(string week_num="", int chCode = 10)
        {
            //Чистим список проверяемых дней
            //chopper1.MyStartupClass.days_to_check.Clear();
            //chopper1.MyStartupClass.variants_to_check.Clear();

            Week curWeek = new Week();

            
            TVWeekType curTvWeek= new TVWeekType();
          
            string curTvWeekNum = getWeekNum(week_num);
            curTvWeek = MyStartupClass.tvWeeks[MyStartupClass.tvWeeks.Length - 1 - Convert.ToInt32(curTvWeekNum)];
            ViewBag.WeekId = curTvWeekNum;

            List<WeekTVDayType> daysOfWeek = getDaysOfWeek(curTvWeek, new[] {chCode}).OrderBy(o => o.TVDate).ToList();
            List<int> availableChannels = new List<int>();
            availableChannels.Add(chCode);            

            //Составляем список каналов, доступных в выбранной неделе
            foreach (int channel in MyStartupClass.concurChannelsArray)
            {
                if (channel != chCode)
                {
                    if (getDaysOfWeek(curTvWeek, new[] {channel}).Count>0)
                    {
                        availableChannels.Add(channel);
                    }
                }
            }
            curWeek.AvailableChannels = availableChannels.ToArray();


            //List<WeekTVDayType> SortedList = daysOfWeek.OrderBy(o => o.TVDate).ToList();
            //EfirType[] efirs = getEfirsByTVday(daysOfWeek[20]);
            curWeek.InjectFrom(curTvWeek);
            curWeek.DaysCount = daysOfWeek.Count();            
            curWeek.Days = daysOfWeek;
            ViewData["daysCount"]= daysOfWeek.Count();
            
            return View(curWeek);
        }

        


        public List<WeekTVDayType> getDaysOfWeek(TVWeekType week, int[] array_channel_codes)
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
            
            //var selectList = new SelectList(weeks, "Value", "Text", MyStartupClass.tvWeeks.Length-1- chopper1.MyStartupClass.selectedID);
                var selectList = new SelectList(weeks, "Value", "Text", MyStartupClass.tvWeeks.Length - 1 - curId);
            
            //var selectList = new SelectList(weeks, "Value", "Text", curId);
            
            ViewData["Weeks1"] = selectList;
            ViewData["ReportType"] = repType;
            ViewBag.Week = weeks;
            return View();

        }

        public ViewResult WeekChosen(string WeekID, string repType, string chCode = "10")
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
            ViewBag.chCode = chCode;

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
            //chopper1.MyStartupClass.days_to_check.Clear();
            //chopper1.MyStartupClass.variants_to_check.Clear();

            Week curWeek = new Week();


            TVWeekType curTvWeek= new TVWeekType();
            /*
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

                    ViewBag.WeekId = (MyStartupClass.getCurrentWeek(weeks) - shift).ToString();
                }
                else
                {
                    if (week_num == "")
                    {
                        curTvWeek = weeks[MyStartupClass.getWeekInWork(weeks)];
                        MyStartupClass.selectedID = MyStartupClass.getWeekInWork(weeks);
                        ViewBag.WeekId = MyStartupClass.getCurrentWeek(weeks).ToString();
                    }
                    else
                    {
                        curTvWeek = weeks[weeks.Length - 1 - Convert.ToInt32(week_num)];
                        ViewBag.WeekId = Convert.ToInt32(week_num).ToString();
                    }
                }

                ViewData["weekName"] = curWeek.Name;

            }
            catch
            {

            }
            */

            string curTvWeekNum = getWeekNum(week_num);
            curTvWeek = MyStartupClass.tvWeeks[MyStartupClass.tvWeeks.Length - 1 - Convert.ToInt32(curTvWeekNum)];
            ViewBag.WeekId = curTvWeekNum;
            
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
        public ActionResult SvodkaText(string bdate = "")
        {
            //Чистим список проверяемых дней
            //chopper1.MyStartupClass.days_to_check.Clear();
            //chopper1.MyStartupClass.variants_to_check.Clear();

            string dateStr = "";
            DateTime weekStart = new DateTime();

            if (Request["bdate"] == null)
            {
                dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");                
                if ((int)DateTime.Parse(dateStr).DayOfWeek != 1)
                {
                    weekStart = DateTime.Parse(dateStr) - TimeSpan.FromDays((int)DateTime.Parse(dateStr).DayOfWeek - 1);
                }
            }
            else
            {
                dateStr = Request["bdate"];
                if (dateStr.Length == 0)
                {
                    dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");                           
                }
                
                //if ((int)DateTime.Parse(dateStr).DayOfWeek != 1)
                //{
                    weekStart = DateTime.Parse(dateStr) - TimeSpan.FromDays((int)DateTime.Parse(dateStr).DayOfWeek - 1);
                //}
            }

            Week curWeek = new Week();
            List<WeekTVDayType> daysToAdd = new List<WeekTVDayType>();

            for (int d = 0; d < 7; d++)
            {
                dateStr = (weekStart + TimeSpan.FromDays(d)).ToString("yyyy-MM-dd");
                for (int i = 0; i < 5; i++)
                {
                    WeekTVDayType newDay = new WeekTVDayType();
                    newDay.TVDate = DateTime.Parse(dateStr);                    
                    newDay.KanalKod = 10 + i;                    
                    newDay.VariantKod = 1;
                    newDay.TVDayRef = "";
                    daysToAdd.Add(newDay);
                }
            }
            curWeek.Days = daysToAdd;

            curWeek.DaysCount = daysToAdd.Count() / 5; //Т.к. каждый день - это ПК+4 орбиты
            return View(curWeek);

        }

        [HttpGet]
        public ActionResult Svodka(string bdate="")
        {
            //Чистим список проверяемых дней
            //chopper1.MyStartupClass.variants_to_check.Clear();
            //chopper1.MyStartupClass.days_to_check.Clear();

            string dateStr = "";

            if (Request["bdate"] == null)
            {
                dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");
            }
            else
            {
                dateStr = Request["bdate"];
                if (dateStr.Length == 0)
                {
                    dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");
                }
            }

            

            Week curWeek = new Week();
            List<WeekTVDayType> daysToAdd = new List<WeekTVDayType>();
            
            for (int i = 0; i < 5; i++)
            {
                WeekTVDayType newDay = new WeekTVDayType();
                newDay.TVDate = DateTime.Parse(dateStr);                
                if (i>0)
                {
                    newDay.KanalKod = 15 - i;
                }
                else
                {
                    newDay.KanalKod = 10 + i;
                }
                
                newDay.VariantKod = 1;
                newDay.TVDayRef = "";
                daysToAdd.Add(newDay);
            }
            curWeek.Days = daysToAdd;
            return View(curWeek);
        }
        public ActionResult Ratings(string bdate = "")
        {
            string dateStr = "";

            if (Request["bdate"] == null)
            {
                dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");
            }
            else
            {
                dateStr = Request["bdate"];
                if (dateStr.Length == 0)
                {
                    dateStr = (DateTime.Today-TimeSpan.FromDays(1)).Date.ToString("yyyy-MM-dd");
                }
            }

            Week curWeek = new Week();
            List<WeekTVDayType> daysToAdd = new List<WeekTVDayType>();

            

            for (int i = 0; i < 6; i++)
            {
                WeekTVDayType newDay = new WeekTVDayType();
                newDay.TVDate = DateTime.Parse(dateStr);
                switch (i)
                {
                    case 0:
                        newDay.KanalKod = 10;
                        break;
                    case 1:
                        newDay.KanalKod = 21;
                        break;
                    case 2:
                        newDay.KanalKod = 40;
                        break;
                    case 3:
                        newDay.KanalKod = 52;
                        break;
                    case 4:
                        newDay.KanalKod = 51;
                        break;
                    case 5:
                        newDay.KanalKod = 53;
                        break;
                }

                

                newDay.VariantKod = 1;
                newDay.TVDayRef = "";
                daysToAdd.Add(newDay);
            }
            curWeek.Days = daysToAdd;
            return View(curWeek);
        }


        [HttpGet]
        //public ActionResult Broadcast(string dateStr="2015-12-17")
        public ActionResult Broadcast_old(string bdate = "", int variantNum=1)
        {
            //uses tvweekdaytype
            //Чистим список проверяемых дней
            //chopper1.MyStartupClass.variants_to_check.Clear();
            //chopper1.MyStartupClass.days_to_check.Clear();

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
                    //Нужно отказываться от недель и брать дни по дате/каналу/варианту
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
        public ActionResult Broadcast(string bdate = "", int variantNum = 1)
        {

            //Чистим список проверяемых дней
            //chopper1.MyStartupClass.variants_to_check.Clear();
            //chopper1.MyStartupClass.days_to_check.Clear();

            string dateStr = "";

            if (Request["bdate"] == null)
            {
                dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");
            }
            else
            {
                dateStr = Request["bdate"];
                if (dateStr.Length == 0)
                {
                    dateStr = DateTime.Today.Date.ToString("yyyy-MM-dd");
                }
            }


            DateTime curDate = DateTime.Parse(dateStr);            
            List<Day> newDays = new List<Day>();
            
            int[] array_channel_codes = new int[5];
            array_channel_codes[0] = 10;
            array_channel_codes[1] = 14;
            array_channel_codes[2] = 13;
            array_channel_codes[3] = 12;
            array_channel_codes[4] = 11;

            ViewBag.WeekId = MyStartupClass.getWeekNumByDate(curDate);


            foreach (int chCode in array_channel_codes)
            {
                newDays.Add(MyStartupClass.getDayByDateAndVariantCode(curDate, variantNum, chCode));
            }

            return View(newDays);

        }

        public ActionResult OrbWeek(string week_num = "", int part_num = 1)
        {
            //Чистим список проверяемых дней
            //chopper1.MyStartupClass.variants_to_check.Clear();

            Week curWeek = new Week();
            
            TVWeekType curTvWeek = new TVWeekType();
  
            string curTvWeekNum = getWeekNum(week_num);
            curTvWeek = MyStartupClass.tvWeeks[MyStartupClass.tvWeeks.Length - 1 - Convert.ToInt32(curTvWeekNum)];
            ViewBag.WeekId = curTvWeekNum;

            List<WeekTVDayType> days = new List<WeekTVDayType>();
            WeekTVDayType chOneDay = new WeekTVDayType();

            int[] array_channel_codes = new int[5];
            array_channel_codes[0] = 10;
            array_channel_codes[1] = 11;
            array_channel_codes[2] = 12;
            array_channel_codes[3] = 13;
            array_channel_codes[4] = 14;

            List<WeekTVDayType> daysOfWeek = getDaysOfWeek(curTvWeek, array_channel_codes).OrderBy(o => o.TVDate).ToList();            
            
            //Рисуем неделю целиком
            daysOfWeek = daysOfWeek.ToList();

            /*
             * Разбиваем неделю на две части. Рисуем в два приема.
            if (part_num == 1)
            {
                daysOfWeek = daysOfWeek.Take(20).ToList();
            }
            else
            {

            }
            */
            curWeek.InjectFrom(curTvWeek);
            curWeek.DaysCount = daysOfWeek.Count()/5;            
            curWeek.Days = daysOfWeek;
            ViewData["daysCount"]= daysOfWeek.Count();
            ViewBag.reportType = "orbity";            
            
        
            return View(curWeek);

        }

        public ActionResult OrbWeekAjax(string week_num = "", int part_num = 1)
        {
            //Чистим список проверяемых дней
            //chopper1.MyStartupClass.variants_to_check.Clear();

            Week curWeek = new Week();

            //Experiments

            TVWeekType curTvWeek = new TVWeekType();
            
            string curTvWeekNum = getWeekNum(week_num);
            curTvWeek = MyStartupClass.tvWeeks[MyStartupClass.tvWeeks.Length - 1 - Convert.ToInt32(curTvWeekNum)];
            ViewBag.WeekId = curTvWeekNum;

            List<WeekTVDayType> days = new List<WeekTVDayType>();
            WeekTVDayType chOneDay = new WeekTVDayType();

            List<WeekTVDayType> daysOfWeek = getDaysOfWeek(curTvWeek, MyStartupClass.fullChannelCodesArray).OrderBy(o => o.TVDate).ToList();

            //Рисуем неделю целиком
            daysOfWeek = daysOfWeek.ToList();

            curWeek.InjectFrom(curTvWeek);
            curWeek.DaysCount = daysOfWeek.Count() / 5;
            curWeek.Days = daysOfWeek;
            ViewData["daysCount"] = daysOfWeek.Count();
            ViewBag.reportType = "orbity";



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
                    //chopper1.MyStartupClass.variants_to_update.RemoveAt(0);                 
                    //dayToUpdateRef = getTVDayReftoUpdate(chopper1.MyStartupClass.variants_to_update[0].TVDayRef);
                    /*
                    for (int i=0;i<chopper1.MyStartupClass.days_to_check.Count();i++)
                    {
                        if (chopper1.MyStartupClass.days_to_check[i].TVDayRef == dayToUpdateRef)
                        {
                            chopper1.MyStartupClass.days_to_check[i].RenderTime = curWc.GetCurrentTime();
                        }
                    }
                    */
                    //Довольно странная версия проверки времени рендера
                    //Сейчас после нахождения хотя бы одного дня для обновления
                    //всем остальным присваивается текущее время для того, чтобы потом
                    //минимальное время было не меньше текущего
                    //***Нужно бы переделать, хотя работает***
                    
                    
                    if (chopper1.MyStartupClass.variants_to_update.Count() == 1)
                    {
                        DateTime curTime = curWc.GetCurrentTime();
                        foreach (Day day in chopper1.MyStartupClass.days_to_check)
                        {
                            day.RenderTime = curTime;
                        }                        
                    }
                    
                     
                }
            }
            
            var y = "data: "+dayToUpdateRef+"\n\n";            
            return Content(y, "text/event-stream");
        }
        public ActionResult CheckDaysOrb()
        {
            //List<TVDayVariantT> variants = new List<TVDayVariantT>();
            TVDayVariantT curVariant = new TVDayVariantT();
            DateTime curDate = DateTime.Now + TimeSpan.FromDays(1);
            foreach (Day day in chopper1.MyStartupClass.days_to_check)
            {
                //curVariant.TVDayRef = day.TVDayRef;
                //curVariant.VariantNumber = day.VariantKod;
                //variants.Add(curVariant);
                if (curDate > day.RenderTime)
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
                    //dayToUpdateRef = chopper1.MyStartupClass.variants_to_update[0].TVDayRef;                                       
                    dayToUpdateRef = getTVDayReftoUpdate(chopper1.MyStartupClass.variants_to_update[0].TVDayRef);
                    chopper1.MyStartupClass.variants_to_update.RemoveAt(0);

                    //Довольно странная версия проверки времени рендера
                    //Сейчас после нахождения хотя бы одного дня для обновления
                    //всем остальным присваивается текущее время для того, чтобы потом
                    //минимальное время было не меньше текущего
                    //***Нужно бы переделать, хотя работает***
                    if (chopper1.MyStartupClass.variants_to_update.Count() == 0)
                    {
                        DateTime curTime = curWc.GetCurrentTime();
                        foreach (Day day in chopper1.MyStartupClass.days_to_check)
                        {
                            day.RenderTime = curTime;
                        }
                    }
                }
            }

            
            var y = "data: " + dayToUpdateRef + "\n\n";
            return Content(y, "text/event-stream");
            
        }
       
        [HttpPost]
        public ActionResult GetZapas()
        {
            return View(MyStartupClass.zapasEfirs);
        }


        [HttpPost]
        public ActionResult UpdateDayStolby()
        {
            string curDayRef= Request["HTTP_DAYID"];
            chopper1.MyStartupClass.lastNewsStart = 0;
            Day curDay = new Day();          
            /*            
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef)
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    curDay = d;
                    break;
                }
            }
            */

            //Находим день и меняем время отрисовки
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef)
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    curDay = d;
                    break;
                }
            }

            //Находим вариант в списке на обновление и убираем его оттуда
            int counter = 0;
            foreach (TVDayVariantT v in chopper1.MyStartupClass.variants_to_update)
            {
                if (v.TVDayRef == curDayRef)
                {
                    break;
                }
                else
                {
                    counter += 1;
                }
            }
            chopper1.MyStartupClass.variants_to_update.RemoveAt(counter);
            
            //Пытаемся работать с вариантами
            /*
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(curDay.TVDate, curDay.KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            var query = new SelectList(curDayVariantsArray);
            ViewData["VariantKod"] = query;
            */
            var query = MyStartupClass.getVariantsSelectList(curDay.TVDate, curDay.KanalKod);
            ViewData["DayVariants"] = query;
            curDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            return View(curDay);
        }




        [HttpPost]
        public ActionResult UpdateDayWeek()
        {
            string curDayRef = Request["HTTP_DAYID"];
            string curVarNum = Request["HTTP_VARNUM"];
            Day curDay = new Day();
            /*
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef)
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    curDay = d;
                    break;
                }
            }
            */
            //Находим день и меняем время отрисовки
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef & d.VariantKod == Convert.ToInt32(curVarNum))
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    d.VariantKod = Convert.ToInt32(curVarNum);
                    d.TVDayRef = curDayRef;
                    curDay = d;
                    break;
                }
            }


            try
            {

                var query = MyStartupClass.getVariantsSelectList(curDay.TVDate, curDay.KanalKod);
                ViewData["DayVariants"] = query;
                /**/
                TVDayVariantParam curParam = new TVDayVariantParam();
                try
                {
                    curParam = curWc.GetVarTVDayParam(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                    curDay.Footers = curParam.Foot2;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
                /**/

                curDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                if (curDay.KanalKod > 0 & curDay.TVDayRef.Left(8) != "dummyRef")
                {
                    TVDayVariantT curVar = new TVDayVariantT();
                    curVar.VariantNumber = curDay.VariantKod;
                    curVar.TVDayRef = curDay.TVDayRef;
                    if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                    {
                        chopper1.MyStartupClass.variants_to_check.Add(curVar);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                int x = 1;
            }

            if (curDay.Efirs.Count() > 0)
            {
                return View(curDay);    
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateDayOrbity()
        {
            //Работа с вариантами

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
            curDay.OrbEfirs = chopper1.Controllers.DayController.getOrbEfirsList(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            return View(curDay);
        }


        [HttpPost]
        public ActionResult UpdateDayBroadcast()
        {
            string curDayRef = Request["HTTP_DAYID"];
            string curVarNum = Request["HTTP_VARNUM"];
            Day curDay = new Day();

            //Находим день и меняем время отрисовки
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef == curDayRef & d.VariantKod == Convert.ToInt32(curVarNum))
                {
                    d.RenderTime = curWc.GetCurrentTime();
                    d.VariantKod = Convert.ToInt32(curVarNum);
                    d.TVDayRef = curDayRef;
                    curDay = d;
                    break;
                }
            }

            /*
            //Находим вариант в списке на обновление и убираем его оттуда
            int counter = 0;
            foreach (TVDayVariantT v in chopper1.MyStartupClass.variants_to_update)
            {
                if (v.TVDayRef == curDayRef)
                {
                    break;
                }
                else
                {
                    counter += 1;
                }
            }
             */ 
            //chopper1.MyStartupClass.variants_to_update.RemoveAt(counter);
            //Пытаемся работать с вариантами
            /*
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(curDay.TVDate, curDay.KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            var query = new SelectList(curDayVariantsArray);
            SelectList selectList = new SelectList(curDayVariants);

            ViewData["DayVariants"] = query;
            ViewData["VariantKod"] = query;
            */

            var query = MyStartupClass.getVariantsSelectList(curDay.TVDate, curDay.KanalKod);
            ViewData["DayVariants"] = query;
            
            curDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);

            if (curDay.KanalKod > 0 & curDay.TVDayRef.Left(8) != "dummyRef")
            {                
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = curDay.VariantKod;
                curVar.TVDayRef = curDay.TVDayRef;
                if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                {
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }

            return View(curDay);
        }


        public static string getWeekNum(string week_num)
        {
            string weekNum = "";

            try
            {
                if (week_num.Left(3) == "cur")
                {
                    int shift = 0;                    
                    shift = Convert.ToInt32(week_num.Right(week_num.Length - 3));
                    weekNum = (MyStartupClass.tvWeeks.Length - 1 - MyStartupClass.getCurrentWeek(MyStartupClass.tvWeeks) + shift).ToString();

                }
                else
                {
                    if (week_num == "")
                    {
                        weekNum = (MyStartupClass.tvWeeks.Length - 1 - MyStartupClass.getCurrentWeek(MyStartupClass.tvWeeks)).ToString();
                    }
                    else
                    {                     
                        weekNum = Convert.ToInt32(week_num).ToString();
                    }
                }                
            }
            catch
            {

            }

            return weekNum;
        }

        public static string getTVDayReftoUpdate(string curRef="")
        {
            string tvDayRef = "";
            DateTime curDate = DateTime.Now;
            //Определяем дату по референсу дня
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDayRef==curRef)
                {
                    curDate = d.TVDate;
                    
                    break;
                }
            }
            //Получаем референс дня для ПК
            foreach (Day d in chopper1.MyStartupClass.days_to_check)
            {
                if (d.TVDate == curDate)
                {
                    if (d.KanalKod==10)
                    {
                        tvDayRef = d.TVDayRef;
                        break;
                    }
                }
            }

            return tvDayRef;
        }

        public FileResult Download(string dayVariantList = "", string repType = "")
        {
            FileStreamResult result = rtf.printReport(dayVariantList: dayVariantList, repType: repType, pointer:"");

            return result;
        }




    }
}
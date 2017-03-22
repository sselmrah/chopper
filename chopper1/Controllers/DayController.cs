using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;
using chopper1.Models;
using chopper1.Controllers;
using Omu.ValueInjecter;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;



namespace chopper1.Controllers
{
    public class DayController : Controller
    {

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            const string culture = "ru-RU";
            CultureInfo ci = CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        private WebСервис1 curWc = MyStartupClass.wc;
        // GET: Day
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConstructDay(WeekTVDayType curDay)
        {
            //--->
            Day newDay = new Day();
            newDay.InjectFrom(curDay);
            CultureInfo russian = new CultureInfo("ru-RU");            
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);            
            newDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            

            

            //Собираем шапку дня из Cap и MemoryDates
            newDay.FullCap += curDay.Cap;
            if (newDay.FullCap.Length>0)
            {
                newDay.FullCap += "\n";
            }
            newDay.FullCap += curWc.GetVarTVDayParam(newDay.TVDate, newDay.KanalKod, newDay.VariantKod).MemoryDates;
            //newDay.FullCap = newDay.FullCap.Replace("#", "\n");

            //<---

            //Добавляем день в список для проверки
            if (newDay.Efirs.Count() > 0)
            {
                newDay.RenderTime = curWc.GetCurrentTime();
                chopper1.MyStartupClass.days_to_check.Add(newDay);
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = newDay.VariantKod;
                curVar.TVDayRef = newDay.TVDayRef;
                if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                {
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }
            //Пытаемся работать с вариантами
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(curDay.TVDate, curDay.KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            var query = new SelectList(curDayVariantsArray);
            ViewData["VariantKod"] = query;


            return PartialView(newDay);            
        }

        public ActionResult SvodkaDay(WeekTVDayType curDay)
        {

            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            newDay.Efirs = getEfirTypeArraySvodka(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            
            return PartialView(newDay);
        }

        public ActionResult RatingsDay(WeekTVDayType curDay)
        {

            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            RatEfirType[] ratEfirs = curWc.GetRatEfirs(curDay.KanalKod,curDay.TVDate);
            if (ratEfirs.Count() == 0)
            {
                EfirType[] efTypes = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                newDay.RatEfirs = getRatEfirTypeArrayNoRatings(efTypes);
            }
            else
            {
                newDay.RatEfirs = getRatEfirTypeArrayRatings(ratEfirs);
            }


            return PartialView(newDay);
        }


        private Efir[] getRatEfirTypeArrayRatings(RatEfirType[] ratEfirs)
        {
            List<Efir> efirs = new List<Efir>();

            foreach(RatEfirType r in ratEfirs)
            {
                Efir newEfir = new Efir();
                newEfir.Beg = r.Beg;
                newEfir.EndTime = r.End;
                TimeSpan ts = r.End - r.Beg;
                newEfir.Timing = Convert.ToInt32(ts.TotalSeconds);
                //newEfir.Timing = r.Timing;
                newEfir.Title = r.Title;
                newEfir.ANR = r.Title;
                newEfir.ProducerCode = "";                               
                newEfir.SellerCode = "";
                newEfir.Age = 0;
                newEfir.AgeCat = "";
                newEfir.TVDayRef = "";
                newEfir.Cap = "";
                newEfir.Foot = "";
                //Рейтинги
                newEfir.DSti = r.DSTI;
                newEfir.DM = r.DM;
                newEfir.DR = r.DR;
                newEfir.RM = r.RM;
                newEfir.RR = r.RR;

                efirs.Add(newEfir);
            }


            return efirs.ToArray();
        }

        private Efir[] getRatEfirTypeArrayNoRatings(EfirType[] ratEfirs)
        {
            List<Efir> efirs = new List<Efir>();

            foreach (EfirType r in ratEfirs)
            {
                Efir newEfir = new Efir();
                newEfir.Beg = r.Beg;
                newEfir.EndTime = r.Beg + TimeSpan.FromSeconds(r.Timing);                
                newEfir.Timing = r.Timing;                
                newEfir.Title = r.Title;
                newEfir.ANR = r.Title;
                newEfir.ProducerCode = r.ProducerCode;
                newEfir.SellerCode = r.SellerCode;
                newEfir.Age = r.Age;
                newEfir.AgeCat = "";
                newEfir.TVDayRef = r.TVDayRef;
                newEfir.Cap = r.Cap;
                newEfir.Foot = r.Foot;
                //Рейтинги
                newEfir.DSti = 0;
                newEfir.DM = 0;
                newEfir.DR = 0;
                newEfir.RM = 0;
                newEfir.RR = 0;

                efirs.Add(newEfir);
            }


            return efirs.ToArray();
        }



        public static EfirType[] getEfirTypeArraySvodka(DateTime TVDate, int KanalKod, int VariantKod = 1)
        {
            planCatDb pDb = new planCatDb();
            pDb.Open(MyStartupClass.curCatConnection);            
            DataTable EfirsDt = pDb.GetEfirsSvodka(TVDate, KanalKod, VariantKod);
            
            List<EfirType> efirTypeList = new List<EfirType>();

            
            List<ITCType> itcList = new List<ITCType>(); 
            int counter = 0;
            foreach (DataRow row in EfirsDt.Rows)
            {
                EfirType newEfir = new EfirType();
                newEfir.Beg = DateTime.Parse(row["BCDateTime"].ToString());
                if (KanalKod==11 | KanalKod ==12)
                {
                    newEfir.Beg -= TimeSpan.FromDays(1);
                }
                /*
                newEfir.Beg = TVDate + TimeSpan.FromSeconds(Convert.ToInt32(row["NormedBegin"]));     
                if (DateTime.Parse(row["BCDateTime"].ToString()).Date==TVDate & (KanalKod==11 | KanalKod==12))
                {
                    newEfir.Beg -= TimeSpan.FromDays(1);
                }
                if (DateTime.Parse(row["BCDateTime"].ToString()).Date > TVDate & (KanalKod == 10 | KanalKod == 14 | KanalKod == 13))
                {
                    newEfir.Beg += TimeSpan.FromDays(1);
                }
                 */ 
                newEfir.Timing = Convert.ToInt32(row["Timing"]);
                newEfir.Title = row["Title"].ToString();
                newEfir.ANR = newEfir.Title;
                newEfir.ProducerCode = row["ProducerCode"].ToString();
                if (newEfir.ProducerCode.Length == 1) newEfir.ProducerCode = "0" + newEfir.ProducerCode;
                if (newEfir.ProducerCode == "04" & newEfir.Title.ToLower().Contains("передача внутри передачи")) newEfir.ProducerCode = "06";
                newEfir.SellerCode = "0"+row["SellerCode"].ToString();
                if (newEfir.SellerCode.Length == 2) newEfir.SellerCode = "0" + newEfir.SellerCode;
                if (row["Age"].ToString() != null)
                {
                    newEfir.Age = Convert.ToInt32(row["Age"]);
                }
                newEfir.TVDayRef = "";
                newEfir.Cap = "";
                newEfir.Foot = "";
                
                if (Convert.ToInt32(newEfir.ProducerCode) == 6 & newEfir.Title.ToLower().Contains("внутри"))
                {
                    ITCType newITC = new ITCType();
                    newITC.Title = "А";
                    newITC.Timing = newEfir.Timing;
                    itcList.Add(newITC);
                }
                if (Convert.ToInt32(newEfir.ProducerCode) == 0 & newEfir.Title.ToLower().Contains("внутри"))
                {
                    ITCType newITC = new ITCType();
                    newITC.Title = "Р";
                    newITC.Timing = newEfir.Timing;
                    newITC.PointCount = 0;
                    itcList.Add(newITC);
                }


                if ((Convert.ToInt32(newEfir.ProducerCode) != 6 & Convert.ToInt32(newEfir.ProducerCode) != 0) | (newEfir.Title.ToLower().Contains("спецпроект")))
                {
                    if (efirTypeList.Count() > 0)
                    {
                        efirTypeList[efirTypeList.Count() - 1].ITC = itcList.ToArray();
                        int itcTiming = 0;
                        if (itcList.Count()>0)
                        {
                            foreach (ITCType i in itcList)
                            {
                                itcTiming += i.Timing;
                            }
                        }
                        efirTypeList[efirTypeList.Count() - 1].Timing += itcTiming;
                    }
                    itcList.Clear();
                    efirTypeList.Add(newEfir);
                }                

                counter += 1;
                if (KanalKod==11)
                {
                    var x = 0;
                }
            }
            EfirType[] efirTypeArray = efirTypeList.ToArray();
            pDb.Close();
            return efirTypeArray;
        }


        public ActionResult BroadcastDay_old(WeekTVDayType curDay)
        {
            //uses weektvdaytype
            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            newDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);

            //Пытаемся работать с вариантами
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(curDay.TVDate, curDay.KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i=0; i<curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            switch (curDay.KanalKod)
            {
                case 10:
                    newDay.ChOneVariants = curDayVariants;
                    break;
                case 11:
                    newDay.Orb1Variants= curDayVariants;
                    break;
                case 12:
                    newDay.Orb2Variants = curDayVariants;
                    break;
                case 13:
                    newDay.Orb3Variants = curDayVariants;
                    break;
                case 14:
                    newDay.Orb4Variants = curDayVariants;
                    break;
            }
            

            var query = new SelectList(curDayVariantsArray);
            SelectList selectList = new SelectList(curDayVariants);
            ViewData["DayVariants"] = query;
            ViewData["VariantKod"] = query;
            

            //Добавляем день в список для проверки
            newDay.RenderTime = curWc.GetCurrentTime();
            if (newDay.KanalKod > 0)
            {
                chopper1.MyStartupClass.days_to_check.Add(newDay);
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = newDay.VariantKod;
                curVar.TVDayRef = newDay.TVDayRef;
                if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                {
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }
            return PartialView(newDay);
        }

        public ActionResult BroadcastDay(Day curDay)
        {
            /*
            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            newDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            */

            //CultureInfo russian = new CultureInfo("ru-RU");
            //curDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            //curDay.DoWRus = char.ToUpper(curDay.DoWRus[0]) + curDay.DoWRus.Substring(1);


            //Пытаемся работать с вариантами
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(curDay.TVDate, curDay.KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            /*
            switch (curDay.KanalKod)
            {
                case 10:
                    curDay.ChOneVariants = curDayVariants;
                    break;
                case 11:
                    curDay.Orb1Variants = curDayVariants;
                    break;
                case 12:
                    curDay.Orb2Variants = curDayVariants;
                    break;
                case 13:
                    curDay.Orb3Variants = curDayVariants;
                    break;
                case 14:
                    curDay.Orb4Variants = curDayVariants;
                    break;
            }
            */

            var query = new SelectList(curDayVariantsArray);
            SelectList selectList = new SelectList(curDayVariants);
            
            ViewData["DayVariants"] = query;
            ViewData["VariantKod"] = query;


            //Добавляем день в список для проверки
            curDay.RenderTime = curWc.GetCurrentTime();

            if (curDay.KanalKod > 0 & curDay.TVDayRef.Left(8) != "dummyRef")
            {
                chopper1.MyStartupClass.days_to_check.Add(curDay);
                
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = curDay.VariantKod;
                curVar.TVDayRef = curDay.TVDayRef;
                if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                {
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
                 
            }
             
            return PartialView(curDay);
        }

        public PartialViewResult BroadcastDay_new(Day newDay)
        {

            //Day newDay = new Day();
            //DateTime dt = DateTime.Parse(dtStr);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = newDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            newDay.Efirs = curWc.GetEfirs(newDay.TVDate.Date, newDay.KanalKod, newDay.VariantKod);

            //Пытаемся работать с вариантами
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(newDay.TVDate.Date, newDay.VariantKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            switch (newDay.VariantKod)
            {
                case 10:
                    newDay.ChOneVariants = curDayVariants;
                    break;
                case 11:
                    newDay.Orb1Variants = curDayVariants;
                    break;
                case 12:
                    newDay.Orb2Variants = curDayVariants;
                    break;
                case 13:
                    newDay.Orb3Variants = curDayVariants;
                    break;
                case 14:
                    newDay.Orb4Variants = curDayVariants;
                    break;
            }
            //var selectList = new SelectList(weeks, "Value", "Text", MyStartupClass.tvWeeks.Length - 1 - curId);

            var query = new SelectList(curDayVariantsArray);
            SelectList selectList = new SelectList(curDayVariants);
            ViewData["VariantKod"] = query;
            ViewData["DayVariants"] = query;


            //Добавляем день в список для проверки
            newDay.RenderTime = curWc.GetCurrentTime();
            if (newDay.KanalKod > 0)
            {
                chopper1.MyStartupClass.days_to_check.Add(newDay);
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = newDay.VariantKod;
                curVar.TVDayRef = newDay.TVDayRef;
                if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                {
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }
            return PartialView(newDay);
        }
        
        public ActionResult ConstructOrbDay(List<WeekTVDayType> curDayList)
        {
            WeekTVDayType curDay = curDayList[0];
    
            Day mainDay = new Day();

            Debug.Print("Начали рисовать день: " + DateTime.Now.ToString("hh:mm:ss"));
            foreach (WeekTVDayType dt in curDayList)
            {
                Day newDay = new Day();                
                TVDayVariantT curVar = new TVDayVariantT();
                if (dt.KanalKod == 10)
                {
                    
                    var results = from o in MyStartupClass.cachedDays
                                  where o.TVDate == dt.TVDate & 
                                  o.VariantKod == dt.VariantKod &
                                  o.KanalKod == dt.KanalKod
                                  select o;
                        
                    if (results.Count()>0)
                    {
                        newDay = results.ToArray()[0];
                        mainDay = results.ToArray()[0];
                    }
                    else
                    {
                        newDay.InjectFrom(dt);

                        CultureInfo russian = new CultureInfo("ru-RU");
                        newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
                        newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);

                        //Собираем шапку дня из Cap и MemoryDates
                        newDay.FullCap += curDay.Cap;
                        if (newDay.FullCap.Length > 0)
                        {
                            newDay.FullCap += "\n";
                        }
                        newDay.FullCap += MyStartupClass.wc.GetVarTVDayParam(newDay.TVDate, newDay.KanalKod, newDay.VariantKod).MemoryDates;
                        //newDay.FullCap = newDay.FullCap.Replace("#", "<br>");


                        if (dt.KanalKod == 10)
                        {
                            newDay.OrbEfirs = getOrbEfirsList(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                            mainDay = newDay;
                        }
                    }
                    newDay.RenderTime = MyStartupClass.wc.GetCurrentTime();
                    if (newDay.KanalKod > 0)
                    {
                        chopper1.MyStartupClass.days_to_check.Add(newDay);
                        curVar.VariantNumber = dt.VariantKod;
                        curVar.TVDayRef = dt.TVDayRef;
                        if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                        {
                            chopper1.MyStartupClass.variants_to_check.Add(curVar);
                        }
                    }

                }
            }
            Debug.Print("Закончили рисовать день: " + DateTime.Now.ToString("hh:mm:ss"));
            return PartialView(mainDay);
        }


        [HttpGet]
        public ActionResult ConstructZapas()
        {
            List<EfirType> zapas = MyStartupClass.zapasEfirs;
            return View(zapas);
        }
        
        [HttpPost]
        public ActionResult AddProgToZapas(string title = "Шаблон", string pureDur="52", string fullCode="00000")
        {
            pureDur = pureDur.Replace(" ", "").Replace("\n", "");
            title = title.Replace("&lt;", "<");
            title = title.Replace("&gt;", ">");
            title = title.Replace("&amp;", "&");
            EfirType curEfir = MyStartupClass.createEfirTypeFromTitleTimingCode(title, pureDur, fullCode);
            MyStartupClass.zapasEfirs.Add(curEfir);
            return View();
        }

        [HttpPost]
        public ActionResult RemoveProgFromZapas(string progId = "")
        {
            if (progId != "")
            {
                foreach (EfirType z in MyStartupClass.zapasEfirs)
                {
                    if (z.Ref == progId)
                    {
                        MyStartupClass.zapasEfirs.Remove(z);
                        break;
                    }
                }
            }            
            return View();
        }

        [HttpPost]
        public ActionResult AddDayByDate(string curDt = "18.07.2016", string data = "")
        {            
            DateTime dt = DateTime.Parse(curDt);
            int varCode = 1;
            Day newDay = MyStartupClass.getDayByDateAndVariantCode(dt, varCode);

            ViewData["DayVariants"] = MyStartupClass.getVariantsList(dt);

            return PartialView(newDay);
        }

        public ActionResult AddDayWrap(Day d, bool isFirstDay = false, bool isLastDay = false)
        {
            ViewData["DayVariants"] = MyStartupClass.getVariantsList(d.TVDate);
            string dayClass = "dayrect ";
            if (isFirstDay) dayClass+="firstDay";
            if (isLastDay) dayClass+="lastDay";
            ViewData["DayClass"] = dayClass;
            return PartialView(d);
        }


        private async Task<Day> createDayAjax(List<WeekTVDayType> curDayList)
        {
            WeekTVDayType curDay = curDayList[0];
            Day mainDay = new Day();

            foreach (WeekTVDayType dt in curDayList)
            {
                Day newDay = new Day();
                TVDayVariantT curVar = new TVDayVariantT();
                newDay.InjectFrom(dt);

                CultureInfo russian = new CultureInfo("ru-RU");
                newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
                newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);

                //Собираем шапку дня из Cap и MemoryDates
                newDay.FullCap += curDay.Cap;
                if (newDay.FullCap.Length > 0)
                {
                    newDay.FullCap += "\n";
                }
                newDay.FullCap += curWc.GetVarTVDayParam(newDay.TVDate, newDay.KanalKod, newDay.VariantKod).MemoryDates;
                //newDay.FullCap = newDay.FullCap.Replace("#", "<br>");


                if (dt.KanalKod == 10)
                {
                    newDay.OrbEfirs = getOrbEfirsList(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                    mainDay = newDay;
                }
                newDay.RenderTime = curWc.GetCurrentTime();
                if (newDay.KanalKod > 0)
                {
                    chopper1.MyStartupClass.days_to_check.Add(newDay);
                    curVar.VariantNumber = dt.VariantKod;
                    curVar.TVDayRef = dt.TVDayRef;
                    if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                    {
                        chopper1.MyStartupClass.variants_to_check.Add(curVar);
                    }
                }
            }
            return mainDay;
        }

        [HttpGet]
        public async Task<ActionResult> ConstructOrbDayAjax(List<WeekTVDayType> curDayList)
        {
            
            var model = await this.createDayAjax(curDayList);            
            return PartialView("ConstructOrbDayAjax", model);
        }

        public ActionResult ConstructNormalTimeScale(bool left)
        {            
            ViewBag.Left = left;
            return PartialView();
        }
        public ActionResult ConstructTimeScale(bool left, int channelCode)
        {
            ViewBag.Left = left;
            return PartialView(channelCode);
        }
        public ActionResult DrawTimeScale(bool left, int channelCode)
        {
            ViewBag.Left = left;
            return PartialView(channelCode);
        }

        public ActionResult StolbyDay(WeekTVDayType curDay)
        {
            chopper1.MyStartupClass.lastNewsStart = 0;
            chopper1.MyStartupClass.totalBlockDur = 0;
            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            newDay.Efirs = curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            //Собираем шапку дня из Cap и MemoryDates
            newDay.FullCap += curDay.Cap;
            
            //newDay.FullCap.Replace("#", "\n");


            //Добавляем день в список для проверки
            if (newDay.Efirs.Count() > 0)
            {
                newDay.RenderTime = curWc.GetCurrentTime();
                chopper1.MyStartupClass.days_to_check.Add(newDay);
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = newDay.VariantKod;
                curVar.TVDayRef = newDay.TVDayRef;
                if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                {
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }
            //Пытаемся работать с вариантами
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(curDay.TVDate, curDay.KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            var query = new SelectList(curDayVariantsArray);
            ViewData["VariantKod"] = query;


            TVDayVariantParam curParam = new TVDayVariantParam();
            try
            {
                curParam = curWc.GetVarTVDayParam(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);                
                newDay.Footers = curParam.Foot2;                
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }



            return PartialView(newDay);
        }

        public ActionResult SvodkaTextDay(WeekTVDayType curDay)
        {
            chopper1.MyStartupClass.lastNewsStart = 0;
            chopper1.MyStartupClass.totalBlockDur = 0;
            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            newDay.Efirs = getEfirTypeArraySvodka(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
            newDay.FullCap = "";
            newDay.Cap = "";
            return PartialView(newDay);
        }

        public ViewResult SelectVariant(SelectList variantList)
        {
                                    
            return View();
        }


        public ViewResult VariantChosen(string VariantKod = "", string curDate = "", string chCode = "", string repType="")
        {

            int curVariant = Convert.ToInt32(VariantKod.Right(VariantKod.Length - 8));
            int KanalKod = Convert.ToInt32(chCode);
            
            DateTime curDt = DateTime.Parse(curDate);
            Day newDay = new Day();
            newDay.Efirs = curWc.GetEfirs(curDt, KanalKod, curVariant);
            //Пытаемся работать с вариантами
            TVDayVariantType[] curDayVariants = curWc.GetDayVariants(curDt, KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            var query = new SelectList(curDayVariantsArray);                        
            ViewData["VariantKod"] = query;



            
            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDt.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
            newDay.KanalKod = KanalKod;
            newDay.VariantKod = curVariant;
            newDay.TVDate = curDt;

            var x = curWc.GetVarTVDayParam(curDt, newDay.KanalKod, newDay.VariantKod);
            newDay.TVDayRef = x.TVDayRef;

            //Собираем шапку дня из Cap и MemoryDates            

            //Добавляем день в список для проверки
            if (newDay.Efirs.Count() > 0)
            {
                
                newDay.RenderTime = curWc.GetCurrentTime();
                chopper1.MyStartupClass.days_to_check.Add(newDay);
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = newDay.VariantKod;
                curVar.TVDayRef = newDay.TVDayRef;
                if (!chopper1.MyStartupClass.variants_to_check.Contains(curVar))
                {
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }




            ViewData["curChCode"] = KanalKod.ToString();
            ViewData["curVariant"] = curVariant.ToString();
            ViewData["curDt"] = curDt.Date.ToString();
            ViewData["repType"] = repType;
            return View(newDay);


        }


        public static Efir[] getOrbEfirsList(DateTime curTVDate, int curKanalKod, int curVariantKod = 1)
        {
            List<EfirType> efirsList = new List<EfirType>();
            List<Efir> orbEfirsList = new List<Efir>();
            TimeSpan timeShift = TimeSpan.FromHours(0);
            //Получили эфиры ПК                        
            efirsList.AddRange(chopper1.MyStartupClass.wc.GetEfirs(curTVDate, curKanalKod, curVariantKod).ToList());
            foreach (EfirType ef1 in efirsList)
            {
                Efir newEfir = new Efir();
                newEfir.InjectFrom(ef1);
                newEfir.OrbCh1 = true;
                orbEfirsList.Add(newEfir);
                newEfir.ChCode = 10;
            }

            //Начали сверять эфиры ПК с эфирами орбит            
            bool foundMatch = false;
            for (int i = 4; i > 0; i--)
            {

                //Почистили список
                efirsList.Clear();
                //Добавили в список эфиры текущей орбиты
                efirsList.AddRange(chopper1.MyStartupClass.wc.GetEfirs(curTVDate, curKanalKod + i, curVariantKod).ToList());
                //Пошли по списку эфиров текущей орбиты
                foreach (EfirType ef2 in efirsList)
                {
                    foundMatch = false;
                    switch (i)
                    {
                        case 1:
                            timeShift = TimeSpan.FromHours(8);
                            ef2.Beg = ef2.Beg + timeShift;
                            break;
                        case 2:
                            timeShift = TimeSpan.FromHours(6);
                            ef2.Beg = ef2.Beg + timeShift;
                            break;
                        case 3:
                            timeShift = TimeSpan.FromHours(4);
                            ef2.Beg = ef2.Beg + timeShift;
                            break;
                        case 4:
                            timeShift = TimeSpan.FromHours(2);
                            ef2.Beg = ef2.Beg + timeShift;
                            break;
                    }
                    double puredur2 = ef2.Timing;
                    foreach(ITCType itc in ef2.ITC)
                    {
                        puredur2 -= itc.Timing;
                    }


                    //Пошли по списку общему списку эфиров дня
                    foreach (Efir ef1 in orbEfirsList)
                    {
                        double puredur1 = ef1.Timing;
                        foreach (ITCType itc in ef1.ITC)
                        {
                            puredur1 -= itc.Timing;
                        }
                        //Если нашли совпадение - ставим метку в имеющемся эфире
                        if (ef1.Beg == ef2.Beg & ef1.Timing == ef2.Timing & puredur1 == puredur2 & ef1.Title == ef2.Title & ef1.ChCode - 10 == Convert.ToInt32(MyStartupClass.getNearestOrb(i)))
                        {
                            ef1.ChCode = i + 10;
                            switch (i)
                            {
                                case 1:
                                    ef1.Orb1 = true;
                                    break;
                                case 2:
                                    ef1.Orb2 = true;
                                    break;
                                case 3:
                                    ef1.Orb3 = true;
                                    break;
                                case 4:
                                    ef1.Orb4 = true;
                                    break;
                            }
                            foundMatch = true;
                            break;
                        }
                    }
                    //Если не нашли совпадения - добавляем эфир в orbEfirList с указанием орбиты
                    if (!foundMatch)
                    {
                        Efir newEfir = new Efir();
                        newEfir.InjectFrom(ef2);
                        switch (i)
                        {
                            case 1:
                                newEfir.Orb1 = true;
                                newEfir.ChCode = 11;
                                break;
                            case 2:
                                newEfir.Orb2 = true;
                                newEfir.ChCode = 12;
                                break;
                            case 3:
                                newEfir.Orb3 = true;
                                newEfir.ChCode = 13;
                                break;
                            case 4:
                                newEfir.Orb4 = true;
                                newEfir.ChCode = 14;
                                break;
                        }
                        orbEfirsList.Add(newEfir);
                    }
                }
            }
            return (orbEfirsList.ToArray());
        }



    }
}
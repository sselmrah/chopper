using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;
using chopper1.Models;
using chopper1.Controllers;
using Omu.ValueInjecter;
using System.Globalization;
using System.Threading;


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

            //Добавляем день в список для проверки
            if (newDay.Efirs.Count() > 0)
            {
                newDay.RenderTime = curWc.GetCurrentTime();
                chopper1.MyStartupClass.days_to_check.Add(newDay);
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = newDay.VariantKod;
                curVar.TVDayRef = newDay.TVDayRef;
                chopper1.MyStartupClass.variants_to_check.Add(curVar);
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
        public ActionResult BroadcastDay(WeekTVDayType curDay)
        {

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
                chopper1.MyStartupClass.variants_to_check.Add(curVar);
            }
            return PartialView(newDay);
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
                chopper1.MyStartupClass.variants_to_check.Add(curVar);
            }
            return PartialView(newDay);
        }
        
        public ActionResult ConstructOrbDay(List<WeekTVDayType> curDayList)
        {
            WeekTVDayType curDay = curDayList[0];
            /*
            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);

            newDay.OrbEfirs = getOrbEfirsList(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);

            //Добавляем день в список для проверки
            //Это наверняка не работает - переписать!!!
            newDay.RenderTime = curWc.GetCurrentTime();
            if (newDay.KanalKod > 0)
            {
                chopper1.MyStartupClass.days_to_check.Add(newDay);
                TVDayVariantT curVar = new TVDayVariantT();                
                foreach (WeekTVDayType dt in curDayList)
                {                                        
                    curVar.VariantNumber = dt.VariantKod;
                    curVar.TVDayRef = dt.TVDayRef;
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }
            */

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
            

                if (dt.KanalKod==10)
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
                    chopper1.MyStartupClass.variants_to_check.Add(curVar);
                }
            }


            return PartialView(mainDay);
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
                chopper1.MyStartupClass.variants_to_check.Add(curVar);
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

                //chopper1.MyStartupClass.variants_to_check.Add(curVar);
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
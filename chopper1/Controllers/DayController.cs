﻿using System;
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
            Day newDay = new Day();
            newDay.InjectFrom(curDay);

            CultureInfo russian = new CultureInfo("ru-RU");
            newDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);

            /*

            List<EfirType> efirsList = new List<EfirType>();
            List<Efir> orbEfirsList = new List<Efir>();
            TimeSpan timeShift = TimeSpan.FromHours(0);
            //Получили эфиры ПК                        
            efirsList.AddRange(curWc.GetEfirs(curDay.TVDate, curDay.KanalKod, 1).ToList());
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
            for (int i = 4; i > 0;i-- )
            {

                //Почистили список
                efirsList.Clear();
                //Добавили в список эфиры текущей орбиты
                efirsList.AddRange(curWc.GetEfirs(curDay.TVDate, curDay.KanalKod+i, 1).ToList());
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
                    //Пошли по списку общему списку эфиров дня
                    foreach (Efir ef1 in orbEfirsList)
                    {
                        //Если нашли совпадение - ставим метку в имеющемся эфире
                        if (ef1.Beg == ef2.Beg & ef1.Timing == ef2.Timing & ef1.Title == ef2.Title & ef1.ChCode-10 == Convert.ToInt32(MyStartupClass.getNearestOrb(i)))
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
            

            //Так было
            //newDay.Efirs = efirsList.ToArray();

            //Так стало
            newDay.OrbEfirs = orbEfirsList.ToArray();
            */


            newDay.OrbEfirs = getOrbEfirsList(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);


            //Добавляем день в список для проверки
            //Это наверняка не работает - переписать!!!
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
            return PartialView(newDay);
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
                    //Пошли по списку общему списку эфиров дня
                    foreach (Efir ef1 in orbEfirsList)
                    {
                        //Если нашли совпадение - ставим метку в имеющемся эфире
                        if (ef1.Beg == ef2.Beg & ef1.Timing == ef2.Timing & ef1.Title == ef2.Title & ef1.ChCode - 10 == Convert.ToInt32(MyStartupClass.getNearestOrb(i)))
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
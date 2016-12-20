using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using chopper1.ws1c;
using chopper1.Models;
using System.Web.Mvc;

using System.Text;
using System.Globalization;
using Omu.ValueInjecter;
using System.Reflection;
using System.IO;
using System.Diagnostics;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace chopper1
{

    public class MyStartupClass
    {
        public static WebСервис1 wc = new WebСервис1();
        public static int selectedID = 2;
        public static TVWeekType[] tvWeeks;
        public static int lastNewsStart = 0;
        public static int totalBlockDur = 0;
        public static List<chopper1.Models.Day> days_to_check = new List<chopper1.Models.Day>();
        public static List<TVDayVariantT> variants_to_check = new List<TVDayVariantT>();
        public static List<TVDayVariantT> variants_to_update = new List<TVDayVariantT>();
        public static string curCatConnection = "PlanCatConnection";
        public static CultureInfo russian = new CultureInfo("ru-RU"); 
        public static int[] fullChannelCodesArray = new int[] { 10, 11, 12, 13, 14 };
        public static int[] concurChannelsArray = new int[] { 10, 21, 40};

        //Cache
        public static List<chopper1.Models.Week> cachedWeeks = new List<chopper1.Models.Week>();
        public static List<chopper1.Models.Day> cachedDays = new List<chopper1.Models.Day>();
        
        //Zapas
        public static List<EfirType> zapasEfirs = new List<EfirType>();

       



        public static void Init()
        {
            DateTime point1 = DateTime.Now;
            wc.Credentials = new System.Net.NetworkCredential("mike", "123");
            try
            {
                wc.Url = "http://plan12r/plan1cw/ws/ws1.1cws";                
                tvWeeks = wc.GetWeeks();                
                selectedID = getWeekInWork(tvWeeks);
            }
            catch
            {
                wc.Url = "http://tsurface/plan1cw/ws/ws1.1cws";
                tvWeeks = wc.GetWeeks();
                selectedID = getWeekInWork(tvWeeks);
                curCatConnection = "TSurfaceCatConnection";
            }
            /*
            DateTime point2 = DateTime.Now;
            tvWeeks.Reverse();
            DateTime point3 = DateTime.Now;
            //cacheDays();
            DateTime point4 = DateTime.Now;
            Debug.Print(point1.ToString("HH:mm:ss"));
            Debug.Print(point2.ToString("HH:mm:ss"));
            Debug.Print(point3.ToString("HH:mm:ss"));
            Debug.Print(point4.ToString("HH:mm:ss"));            
            * */
        }



        private static void initDayAccess()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            DayAccess newDA = new DayAccess();
            
            
            context.DaysAccess.Add(newDA);
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }

        private static void cacheDays()
        {
            for (int j = -7; j<22;j++)
            {
                foreach (int chCode in fullChannelCodesArray)
                {
                    Day curDay = new Day();
                    DateTime curDate = DateTime.Now + TimeSpan.FromDays(j);
                    curDate = curDate.Date;
                    TVDayVariantType[] curDayVariants = wc.GetDayVariants(curDate, chCode);
                        foreach (TVDayVariantType var in curDayVariants)
                        {
                            curDay = getDayByDateAndVariantCode(curDate, var.VariantCode, chCode);
                            
                            if (chCode == 10)
                            {        
                                curDay.OrbEfirs = chopper1.Controllers.DayController.getOrbEfirsList(curDate, chCode, var.VariantCode);
                            }
                            cachedDays.Add(curDay);
                        }                             
                }
            }
        }


        public static SelectList getVariantsSelectList(DateTime dt, int chCode)
        {

            TVDayVariantType[] curDayVariants = wc.GetDayVariants(dt, chCode);
            

            if (curDayVariants.Length > 0)
            {
                string[] curDayVariantsArray = new string[curDayVariants.Length];
                for (int i = 0; i < curDayVariants.Length; i++)
                {                    
                    curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
                    
                }
                var query = new SelectList(curDayVariantsArray);                                        
                return query;
            }
            else
            {
                curDayVariants = wc.GetDayVariants(dt, chCode);
                string[] curDayVariantsArray = new string[1];
                curDayVariantsArray[0] = "Вариант 1";
                var query = new SelectList(curDayVariantsArray);
                return query;
            }
        }

        

        public static TVDayVariantT[] getTVDayVariantTArray(string[] days, string[] vars)
        {
            TVDayVariantT[] res;
            List<TVDayVariantT> varList= new List<TVDayVariantT>();
            for (int i = 0; i < days.Length; i++)
            {
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = Convert.ToInt32(vars[i]);
                curVar.TVDayRef = days[i];
                varList.Add(curVar);
            }
            res = varList.ToArray();

            return res;
        }

        public static int getWeekInWork(TVWeekType[] tvWeeks)
        {
            int curWeekId = 0;
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - DateTime.Now.Date <= TimeSpan.FromDays(13))
                {
                    curWeekId = i;
                    break;
                }
            }

            return curWeekId;
        }
        public static int getCurrentWeek(TVWeekType[] tvWeeks)
        {
            int curWeekId = 0;
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - DateTime.Now.Date <= TimeSpan.FromDays(0))
                {
                    curWeekId = i;
                    break;
                }
            }

            return curWeekId;
        }

        public static int getWeekNumByDate(DateTime curDate)
        {
            int curWeekId = 0;
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - curDate.Date <= TimeSpan.FromDays(0))
                {
                    curWeekId = tvWeeks.Length - i - 1;
                    break;
                }
            }

            return curWeekId;
        }

        public static string getWeekRefByDate(DateTime curDate)
        {
            string weekRef = "";
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - curDate.Date <= TimeSpan.FromDays(0))
                {
                    weekRef = tvWeeks[i].Ref;
                    break;
                }
            }

            return weekRef;
        }


        public static int getOrbNumberByChannelCode(int chCode)
        {
            int orbNum = chCode - 10;
            return orbNum;
        }


        public static int getNearestOrb(int orbNum)
        {
            int nearestOrb = 0;
            switch (orbNum)
            {
                case 1:
                    nearestOrb = 2;
                    break;
                case 2:
                    nearestOrb = 3;
                    break;
                case 3:
                    nearestOrb = 4;
                    break;
                case 4:
                    nearestOrb = 0;
                    break;
            }
            return nearestOrb;
        }

        public static Day getDayByDateAndVariantCode(DateTime curDate, int curVar, int chCode = 10)
        {
            Day curDay = new Day();

            curDay.KanalKod = chCode;
            curDay.VariantKod = curVar;
            curDay.TVDate = curDate;

            curDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
            curDay.DoWRus = char.ToUpper(curDay.DoWRus[0]) + curDay.DoWRus.Substring(1);

            TVDayVariantType[] v = wc.GetDayVariants(curDate, chCode);
            if (v.Count() > 0)
            {
                curDay.Efirs = wc.GetEfirs(curDate, chCode, curVar);
                TVDayVariantParam curParam = new TVDayVariantParam();
                try
                {
                    curParam = wc.GetVarTVDayParam(curDate, chCode, curVar);
                    curDay.TVDayRef = curParam.TVDayRef;
                    if (curDay.TVDayRef.Length == 0)
                    {
                        curDay.TVDayRef = "dummyRef";
                        curDay.TVDayRef += curDate.Date.ToString("yyyyMMdd");
                        curDay.TVDayRef += "var";
                        curDay.TVDayRef += curVar.ToString();
                    }
                    curDay.Cap = curParam.Cap;
                    curDay.Foot = curParam.Foot;

                    curDay.Footers = curParam.Foot2;                                                          

                    curDay.FullCap += curDay.Cap;
                    if (curDay.FullCap.Length > 0)
                    {
                        curDay.FullCap += "\n";
                    }
                    curDay.FullCap += curParam.MemoryDates;
                }
                catch
                {

                }
                
            
            }
            else
            {
                curDay.TVDayRef = "dummyRef";
                curDay.TVDayRef += curDate.Date.ToString("yyyyMMdd");
                curDay.TVDayRef += "var";
                curDay.TVDayRef += curVar.ToString();
            }
            return curDay;
        }



        public static SelectList getVariantsList(DateTime TVDate, int KanalKod = 10)
        {            
            //Пытаемся работать с вариантами
            TVDayVariantType[] curDayVariants = wc.GetDayVariants(TVDate, KanalKod);
            string[] curDayVariantsArray = new string[curDayVariants.Length];
            for (int i = 0; i < curDayVariants.Length; i++)
            {
                curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
            }
            var query = new SelectList(curDayVariantsArray);
            SelectList result = query;
            return result;
        }

        public static string getRandomRef(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        /*
        public static string getRandomRef()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
        */

        public static Tuple<int, int, int, int, int, int,int> ads_int_new(int timing)
        {
            //По чистому хронометражу, определяем количество рекламы/анонсов
            //1 - реклама
            //2 - точки
            //3 - анонсы
            //4 - чистый хронометраж (в минутах)
            //5 - общий хронометраж (часы)
            //6 - общий хронометраж (минуты)
            //7 - общий хронометраж (секунды)
            


            int hours = 0;
            int minutes = 0;
            int seconds = timing;

            while (seconds >= 60)
            {
                minutes += 1;
                seconds -= 60;
            }

            while (minutes >= 60)
            {
                hours += 1;
                minutes -= 60;
            }

            //int hours = Convert.ToInt32(timing.Left(2));
            //int minutes = Convert.ToInt32(timing.Substring(3, 2));
            int duration = hours * 60*60 + minutes*60 + seconds;

            int r = 0;
            int t = 0;
            int a = 0;

            int temp_dur = duration;
            bool odd = true;
            while (temp_dur > 0)
            {
                if (odd == true && t > 0)
                {
                    r += 2*60;
                    temp_dur += 2*60;
                }
                if (odd == false)
                {
                    r += 2*60;
                    t += 1;
                    a += 1*60;
                    temp_dur += 3*60;
                }
                odd = !odd;
                temp_dur -= 15*60;

            }


            temp_dur = duration + r + a;
            //Странная поправка на ветер
            if (temp_dur < 120*60)
                r += 2*60;
            //Добавляем раз в час на анонсы для новостей часа
            a += Convert.ToInt32(Math.Floor(Convert.ToDouble(temp_dur / (60*60))))*60;
            //Проверяем количество точек
            if (Convert.ToInt32(r/60) - t * 4 > 0) t += 1;

            //Для коротких передач делаем на 1 анонс больше, чем точек
            while (Convert.ToInt32(a/60) <= t) a += 1*60;

            /*minutes += Convert.ToInt32(r);
            minutes += Convert.ToInt32(a);
             */
            seconds += r;
            seconds += a;
            while (seconds >= 60)
            {
                minutes += 1;
                seconds -= 60;
            }
            while (minutes >= 60)
            {
                hours += 1;
                minutes -= 60;
            }

            return Tuple.Create(r, t, a, duration, hours, minutes, seconds);

        }
        /*
        public static double time_to_minutes(string timing)
        {
            double hours = 0;
            double minutes = 0;
            if (timing.IndexOf("N") < 0)
            {
                if (timing.IndexOf(":") >= 0)
                {
                    hours = Convert.ToInt32(timing.Left(timing.IndexOf(":")));
                    minutes = Convert.ToInt32(timing.Substring(timing.IndexOf(":") + 1, timing.Length - timing.IndexOf(":") - 1));
                }
                else
                {
                    minutes = Convert.ToInt32(timing);
                }
            }
            double total_minutes = hours * 60 + minutes;
            return total_minutes;
        }
        */


        public static EfirType createEfirTypeFromTitleTimingCode(string title, string pureDur, string fullCode)
        {
            
            TimeSpan pureDurTs = TimeSpan.FromMinutes(Convert.ToDouble(pureDur));
            
            int pureDurInt = pureDurTs.Hours * 60 * 60 + pureDurTs.Minutes * 60 + pureDurTs.Seconds;
            title = title.Replace("<", "").Replace(">", "");

            EfirType curEfir = new EfirType();
            curEfir.ANR = title;
            curEfir.Title = title;
            curEfir.ProducerCode = fullCode.Left(2);
            curEfir.SellerCode = fullCode.Right(3);

            curEfir.Ref = MyStartupClass.getRandomRef(16);


            Tuple<int, int, int, int, int, int, int> rTemp = MyStartupClass.ads_int_new(pureDurInt);
            ITCType r = new ITCType();
            ITCType a = new ITCType();
            if (rTemp.Item1 > 0)
            {
                r.Title = "Р";
                r.PointCount = rTemp.Item2;
                r.Timing = rTemp.Item1;
            }
            if (rTemp.Item3 > 0)
            {
                a.Title = "А";
                a.PointCount = 0;//Convert.ToInt32(rTemp.Item2);
                a.Timing = rTemp.Item3;
            }
            ITCType[] rr = new ITCType[] { r, a };
            curEfir.ITC = rr;
            curEfir.Timing = Convert.ToInt32(rTemp.Item5 * 60 * 60 + rTemp.Item6 * 60 + rTemp.Item7);
            return curEfir;
        }

        public static Efir getRTA(int timing, ITCType[] ITCs)
        {
            Efir newEfir = new Efir();
            newEfir.Timing = timing;
            //Получаем общий хронометраж рекламы и анонсов (в секундах) + количество точек + чистый хронометраж
            int r = 0;
            int r99 = 0;
            int sr = 0;
            int tsr = 0;
            int t = 0;
            int t99 = 0;
            int st = 0;
            int a = 0;
            int ta = 0;
            int aITC = 0;
            int taITC = 0;
            int v = 0;
            int tv = 0;
            string ITCname = "";

            if (ITCs != null)
            {

                foreach (ITCType itc in ITCs)
                {
                    /*
               if (itc.Title == "Р")
               {
                   r += itc.Timing;
                   t += itc.PointCount;
               }
               if (itc.Title == "Р99")
               {
                   r99 += itc.Timing;
                   t99 += itc.PointCount;
               }
               if (itc.Title == "СР")
               {
                   sr += itc.Timing;
                   st += itc.PointCount;
               }
               if (itc.Title == "А")
               {
                   a += itc.Timing;
               }

           }
           */
                    switch (itc.Title)
                    {
                        case "Р":
                            r += itc.Timing;
                            t += itc.PointCount;
                            break;
                        case "Р99":
                            r99 += itc.Timing;
                            t99 += itc.PointCount;
                            break;
                        case "СР":
                            sr += itc.Timing;
                            tsr += itc.PointCount;
                            break;
                        case "В":
                            v += itc.Timing;
                            tv += itc.PointCount;
                            break;
                        case "А":
                            a += itc.Timing;
                            ta += itc.PointCount;
                            break;
                        default:
                            aITC += itc.Timing;
                            taITC += itc.PointCount;
                            ITCname = itc.Title;
                            break;
                    }
                }

            }
            newEfir.R = r;
            newEfir.R99 = r99;
            newEfir.Sr = sr;
            newEfir.Tsr = tsr;
            newEfir.T = t;
            newEfir.T99 = t99;
            newEfir.St = st;
            newEfir.A = a;
            newEfir.V = v;
            newEfir.Tv = tv;
            newEfir.AnotherITC = aITC;
            newEfir.TanotherITC = taITC;
            newEfir.AnotherITCName = ITCname;

            if (r + r99 + sr + a + aITC+ v == 0)
            {
                newEfir.PureDur = newEfir.Timing;
            }
            else
            {
                if (newEfir.Timing == r + r99 + sr + a + aITC+v)
                {
                    newEfir.PureDur = newEfir.Timing;
                }
                else
                {/*
                    newEfir.PureDur = newEfir.Timing - r - r99 - sr - a;
                  */
                        newEfir.PureDur = newEfir.Timing - r - r99 - sr - a - aITC-v;
                }
            }
            return newEfir;
        }



    }

   

}
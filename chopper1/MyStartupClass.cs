using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;
using chopper1.Models;
using System.Web.Mvc;


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

        public static int[] fullChannelCodesArray = new int[] { 10, 11, 12, 13, 14 };


        public static void Init()
        {
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
            tvWeeks.Reverse();
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
                //SelectList selectList = new SelectList(curDayVariants);
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
            

            TVDayVariantType[] v = wc.GetDayVariants(curDate, chCode);
            if (v.Count() > 0)
            {
                curDay.Efirs = wc.GetEfirs(curDate, chCode, curVar);

                TVDayVariantParam curParam = wc.GetVarTVDayParam(curDate, chCode, curVar);
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
                curDay.FullCap += curDay.Cap;
                if (curDay.FullCap.Length > 0)
                {
                    curDay.FullCap += "\n";
                }
                curDay.FullCap += curParam.MemoryDates;
            
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

        



    }

   

}
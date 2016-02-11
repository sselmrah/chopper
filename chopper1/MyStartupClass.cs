using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;

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
            }
            tvWeeks.Reverse();
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

    }

}
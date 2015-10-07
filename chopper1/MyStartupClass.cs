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

        public static void Init()
        {
            wc.Credentials = new System.Net.NetworkCredential("mike", "123");
            try
            {
                
                tvWeeks = wc.GetWeeks();
                selectedID = getWeekInWork(tvWeeks);
            }
            catch
            {
                //Пока ничего
            }
        }
        public static int getWeekInWork(TVWeekType[] tvWeeks)
        {
            int curWeekId=0;
            for (int i = 0; i < tvWeeks.Length;i++ )
            {
                if (tvWeeks[i].BegDate.Date-DateTime.Now.Date<= TimeSpan.FromDays(13))
                {
                    curWeekId = i;
                    break;
                }
            }

            return curWeekId;
        }

    }

}
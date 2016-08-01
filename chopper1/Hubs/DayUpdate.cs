using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
//SignalR
using System.Threading;
using System.Web.Hosting;
using chopper1.Models;
using chopper1.ws1c;


namespace chopper1.Hubs
{
    public class DayUpdate : Hub
    {
        chopper1.ws1c.WebСервис1 curWc = MyStartupClass.wc;
        public string getServerTime()
        {
            string curTime = curWc.GetCurrentTime().ToString();
            return curTime;
        }

        public async Task Subscribe(string group)
        {            
            await Groups.Add(Context.ConnectionId, group);            
        }

        public void LeaveGroup(string tvDayRef)
        {
            this.Groups.Remove(Context.ConnectionId, tvDayRef);            
        }

        public void print(string dayVariantList = "", string repType = "Broadcast", string pointer = "2016-07-06", bool pdf = false, bool word = true, bool print = false)
        {
            rtf.printReport(dayVariantList,repType, pointer, pdf, word, print);
        }
        public static void addProg(string title = "Шаблон", string pureDur = "15", string fullCode = "00000")
        {
            /*
            TimeSpan pureDurTs = TimeSpan.Parse(pureDur);
            int pureDurInt = pureDurTs.Hours * 60*60 + pureDurTs.Minutes*60 + pureDurTs.Seconds;
            title = title.Replace("<", "").Replace(">", "");

            EfirType curEfir = new EfirType();
            curEfir.ANR = title;
            curEfir.Title = title;
            //curEfir.Timing = pureDurInt;            
            curEfir.ProducerCode = fullCode.Left(2);
            curEfir.SellerCode = fullCode.Right(2);

            
            


            curEfir.Ref = MyStartupClass.getRandomRef(16);

            //Дописать расчет рекламы
            //Дописать расчет тайминга


            Tuple<int, int, int, int, int, int, int> rTemp = MyStartupClass.ads_int_new(pureDurInt);
            ITCType r = new ITCType();
            ITCType a = new ITCType();
            if (rTemp.Item1>0)
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
            ITCType[] rr = new ITCType[] { r,a };
            curEfir.ITC = rr;
            curEfir.Timing = Convert.ToInt32(rTemp.Item5 * 60 * 60 + rTemp.Item6 * 60 + rTemp.Item7);
            */
            EfirType curEfir = MyStartupClass.createEfirTypeFromTitleTimingCode(title, pureDur, fullCode);





            MyStartupClass.zapasEfirs.Add(curEfir);
        }


        //public void updateDay(string[] dayIds, string[] dayVars, string timeStampStr)
        public async Task updateDay(string[] dayIds, string[] dayVars, string timeStampStr)
        {            
            TVDayVariantT[] varList = MyStartupClass.getTVDayVariantTArray(dayIds, dayVars);
            DateTime dt = DateTime.Parse(timeStampStr);
            DateTime serverDt = curWc.GetCurrentTime();
            /*
            if (serverDt-dt<=TimeSpan.FromSeconds(6))
            {
                serverDt = dt;
            }
             */ 
            TVDayVariantT[] rez = curWc.CheckVariants(varList, dt);
            //Queue<TVDayVariantT> queue = new Queue<TVDayVariantT>();

            if (rez.Count() > 0)
            {
                foreach (TVDayVariantT d in rez)
                {                    
                    //Clients.All.broadcastMessage(d.TVDayRef, serverDt.ToString());
                    await Clients.Group(d.TVDayRef).broadcastMessage(d.TVDayRef, d.VariantNumber.ToString(), serverDt.ToString());                    
                }
            }

            
        }

        public class BackgroundTimer : IRegisteredObject
        {
            chopper1.ws1c.WebСервис1 curWc = MyStartupClass.wc;
            private readonly IHubContext _dUpdateHub;
            private Timer _timer;


            public BackgroundTimer()
            {
                _dUpdateHub = GlobalHost.ConnectionManager.GetHubContext<DayUpdate>();
                StartTimer();
            }
            private void StartTimer()
            {
                _timer = new Timer(BroadcastTVDayRefsToUpdate, null, 5000, 5000);
            }

            private void BroadcastTVDayRefsToUpdate(object state)
            {
                try
                {
                    DateTime serverDt = curWc.GetCurrentTime() - TimeSpan.FromSeconds(6);
                    TVDayVariantT[] rez = curWc.CheckVariants(MyStartupClass.variants_to_check.ToArray(), serverDt);
                    Debug.Print(serverDt.ToString("HH:mm:SS"));
                    if (rez.Count() > 0)
                    {
                        foreach (TVDayVariantT d in rez)
                        {
                            _dUpdateHub.Clients.Group(d.TVDayRef).broadcastMessage(d.TVDayRef, d.VariantNumber.ToString(), serverDt.ToString());
                        }
                    }
                }
                catch
                {                    
                    int i = 0;
                }


            }
            
            
            private void BroadcastUptimeToClients(object state)
            {
                DateTime uptime = DateTime.Now;

                _dUpdateHub.Clients.All.internetUpTime(uptime.ToString());
            }

            public void Stop(bool immediate)
            {
                _timer.Dispose();
                HostingEnvironment.UnregisterObject(this);
            }
        }


    }
}
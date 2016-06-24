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
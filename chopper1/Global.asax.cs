using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Hosting;
using System.Data.Entity;
using chopper1.Models;
using System.Globalization;

namespace chopper1
{    
    public class MvcApplication : System.Web.HttpApplication
    {
        //Initializing web-service for future use
        
        
        protected void Application_Start()
        {
            CultureInfo info = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
            info.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
            System.Threading.Thread.CurrentThread.CurrentCulture = info;
            DateTime orbSwitchDate = DateTime.Parse("25.12.2018");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);            
            Database.SetInitializer<ApplicationDbContext>(null);
            MyStartupClass.Init();
            HostingEnvironment.RegisterObject(new chopper1.Hubs.DayUpdate.BackgroundTimer());
        }   
        
    }
}

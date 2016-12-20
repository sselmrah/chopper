﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Hosting;
using System.Data.Entity;
using chopper1.Models;

namespace chopper1
{    
    public class MvcApplication : System.Web.HttpApplication
    {
        //Initializing web-service for future use
        
        
        protected void Application_Start()
        {
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

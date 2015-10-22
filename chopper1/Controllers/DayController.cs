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
            
            return PartialView(newDay);
        }
        public ActionResult ConstructNormalTimeScale(bool left)
        {            
            ViewBag.Left = left;
            return PartialView();
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
            newDay.RenderTime = curWc.GetCurrentTime();
            chopper1.MyStartupClass.days_to_check.Add(newDay);
            TVDayVariantT curVar = new TVDayVariantT();            
            curVar.VariantNumber = newDay.VariantKod;
            curVar.TVDayRef = newDay.TVDayRef;
            chopper1.MyStartupClass.variants_to_check.Add(curVar);
            return PartialView(newDay);
        }

    }
}
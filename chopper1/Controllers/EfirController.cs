using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;
using chopper1.Models;
using Omu.ValueInjecter;



namespace chopper1.Controllers
{
    public class EfirController : Controller
    {

        
        private WebСервис1 curWc = MyStartupClass.wc;
        // GET: Day
        public ActionResult Index()
        {
            return View();
        }

       
       

        public ActionResult ConstructEfir(EfirType curEfir, DateTime curDay)
        {
            
            Efir newEfir = new Efir();
            newEfir.InjectFrom(curEfir);
            
            //Получаем общее время рекламы / анонсов + чистый хронометраж
            if (newEfir.ITC.Count() > 0)
            {
                newEfir.getRTA(newEfir.ITC);
            }            
            //Получаем время окончания
            newEfir.EndTime = newEfir.Beg + TimeSpan.FromSeconds(newEfir.Timing);
            //Проверяем, укладываемся ли в текущий день
            if (newEfir.Beg.Date==curDay.Date)
            {
                newEfir.IsNextDay = false;
            }
            else
            {
                newEfir.IsNextDay = true;
            }
            //Назначаем высоту в пикселях: 1 минута = 1px
            newEfir.InitHeight = newEfir.Timing / 60;            
            
            return PartialView(newEfir);
        }
    }
}
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
            //Стандартный размер шрифта
            newEfir.FontSize = 9;
            //Обрабатываем ключи
            TitleKeys(newEfir);
            //Получаем общее время рекламы / анонсов + чистый хронометраж
            //if (newEfir.ITC.Count() > 0)
            //{
                newEfir.getRTA(newEfir.ITC);
            //}
            //Проверяем, не новостной ли это эфир
            newEfir.IsNews = false;
            if ((newEfir.ANR.ToUpper().Contains("ВОСКРЕСНОЕ \"ВРЕМЯ\"") || newEfir.ANR.ToUpper().Contains("НОВОСТИ") || newEfir.ANR.ToUpper().Contains("\"ВРЕМЯ\""))) 
            {
                newEfir.IsNews = true;
            }
            //Получаем время окончания
            newEfir.EndTime = newEfir.Beg + TimeSpan.FromSeconds(newEfir.Timing);
            //Проверяем, укладываемся ли в текущий день
            if (newEfir.Beg.Date == curDay.Date)
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

        public ActionResult StolbyEfir(EfirType curEfir, DateTime curDay)
        {

            Efir newEfir = new Efir();
            newEfir.InjectFrom(curEfir);
            //Стандартный размер шрифта
            newEfir.FontSize = 9;
            //Обрабатываем ключи
            TitleKeys(newEfir);
            //Получаем общее время рекламы / анонсов + чистый хронометраж
            //if (newEfir.ITC.Count() > 0)
            //{
                newEfir.getRTA(newEfir.ITC);
            //}



                

            //Проверяем, укладываемся ли в текущий день
            if (newEfir.Beg.Date == curDay.Date)
            {
                newEfir.IsNextDay = false;
            }
            else
            {
                newEfir.IsNextDay = true;
            }
            //Проверяем, не новостной ли это эфир
            newEfir.IsNews = false;
            if ((newEfir.ANR.ToUpper().Contains("ВОСКРЕСНОЕ \"ВРЕМЯ\"") || newEfir.ANR.ToUpper().Contains("НОВОСТИ") || newEfir.ANR.ToUpper().Contains("\"ВРЕМЯ\"")) &! newEfir.ANR.ToUpper().Contains("СПОРТА"))
            {
                newEfir.IsNews = true;
                int thisNewsStart;
                if (newEfir.IsNextDay)
                {
                    thisNewsStart = Convert.ToInt32(newEfir.Beg.TimeOfDay.TotalSeconds) + 24 * 60 * 60;
                }
                else
                {
                    thisNewsStart = Convert.ToInt32(newEfir.Beg.TimeOfDay.TotalSeconds);
                }

                if (chopper1.MyStartupClass.lastNewsStart == 0)
                {
                    chopper1.MyStartupClass.lastNewsStart = thisNewsStart;                    
                    chopper1.MyStartupClass.totalBlockDur = newEfir.Timing;
                    //Дописать обнуление lastnewsstart
                }
                else
                {
                    newEfir.Nakladka = thisNewsStart - chopper1.MyStartupClass.lastNewsStart - chopper1.MyStartupClass.totalBlockDur;
                    chopper1.MyStartupClass.lastNewsStart = thisNewsStart;                    
                    chopper1.MyStartupClass.totalBlockDur = newEfir.Timing;
                }
            }   
            else
            {
                chopper1.MyStartupClass.totalBlockDur += newEfir.Timing;
            }
            
            //Получаем время окончания
            newEfir.EndTime = newEfir.Beg + TimeSpan.FromSeconds(newEfir.Timing);

            //Назначаем высоту в пикселях: 1 минута = 1px
            newEfir.InitHeight = newEfir.Timing / 60;


            return PartialView(newEfir);
        }


        private void TitleKeys(Efir curEfir)
        {
            if (curEfir.ANR.ToLower().Contains("\\b"))
            {
                curEfir.ANR = curEfir.ANR.Replace("\\b0", "");
                curEfir.ANR = curEfir.ANR.Replace("\\b1", "");
                curEfir.ANR = curEfir.ANR.Replace("\\b", "");
                curEfir.Bold = true;
            }
            if (curEfir.ANR.ToLower().Contains("\\i"))
            {
                curEfir.ANR = curEfir.ANR.Replace("\\i", "");
                curEfir.Italic = true;
            }
            if (curEfir.ANR.ToLower().Contains("$Ш"))
            {
                curEfir.ANR = curEfir.ANR.Replace("\\i", "");
                curEfir.Italic = true;
            }


            string cur_key;
            if (curEfir.ANR.Contains("$Ш") || curEfir.ANR.Contains("$Х") || curEfir.ANR.Contains("$X") || curEfir.ANR.Contains("$C") || curEfir.ANR.Contains("$С") || curEfir.ANR.Contains("$Ц"))
            {
                while (curEfir.ANR.IndexOf("$") >= 0)
                {
                    //Размер шрифта
                    if (curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "Ш")
                    {
                        cur_key = curEfir.ANR.Substring(curEfir.ANR.IndexOf("$"), 3);
                        curEfir.FontSize = Convert.ToInt32(cur_key.Right(1)) + 1;
                        curEfir.ANR = curEfir.ANR.Replace(cur_key, "");
                    }
                    //Загадочный ключ
                    if (curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "X" || curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "Х")
                    {
                        cur_key = curEfir.ANR.Substring(curEfir.ANR.IndexOf("$"), 2);
                        curEfir.Reserv = true;
                        curEfir.ANR = curEfir.ANR.Replace(cur_key, "");
                    }
                    //Заливка
                    if (curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "C" || curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "С")
                    {

                        cur_key = curEfir.ANR.Substring(curEfir.ANR.IndexOf("$"), 4);
                        //Без нюансов, так сказать...                        
                        curEfir.GrayScale = Convert.ToInt32(cur_key.Right(cur_key.Length - 2));
                        curEfir.ANR = curEfir.ANR.Replace(cur_key, "");
                    }

                }
            }
        }
    }
}
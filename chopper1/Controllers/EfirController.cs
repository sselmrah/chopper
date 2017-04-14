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


        public ActionResult ConstructRatEfir(Efir curEfir, DateTime curDay, int chCode = 10, bool useTitle = false)
        {
         //Можно, наверно, все почистить, т.к. сразу передаем сюда готовый Efir
            Efir newEfir = new Efir();
            newEfir = curEfir;
            //Ставим код канала
            newEfir.ChCode = chCode;

            //Стандартный размер шрифта
            newEfir.FontSize = 9;
            //Обрабатываем ключи
            //TitleKeys(newEfir);
            //Получаем общее время рекламы / анонсов + чистый хронометраж
            //if (newEfir.ITC.Count() > 0)
            //{
            //newEfir.getRTA(newEfir.ITC);
            //}
            //Проверяем, не новостной ли это эфир
            newEfir.IsNews = false;
            //if ((newEfir.ANR.ToUpper().Contains("ВОСКРЕСНОЕ \"ВРЕМЯ\"") || newEfir.ANR.ToUpper().Contains("НОВОСТИ") || newEfir.ANR.ToUpper().Contains("\"ВРЕМЯ\""))) 
            if (newEfir.ProducerCode == "04")
            {
                newEfir.IsNews = true;
            }

            //Выделяем другие нужные нам типы эфиров. Пока - спорт
            newEfir.IsHighlighted = false;
            if (newEfir.ProducerCode == "24")
            {
                newEfir.IsHighlighted = true;
            }
            //Получаем время окончания
            //newEfir.EndTime = newEfir.Beg + TimeSpan.FromSeconds(newEfir.Timing);
            //Проверяем, укладываемся ли в текущий день
            if (newEfir.Beg.Date == curDay.Date)
            {
                newEfir.IsNextDay = false;
                newEfir.IsPrevDay = false;
            }
            else
            {
                if (newEfir.Beg.Date > curDay.Date)
                {
                    newEfir.IsNextDay = true;
                    newEfir.IsPrevDay = false;
                }
                else
                {
                    newEfir.IsNextDay = false;
                    newEfir.IsPrevDay = true;
                }
            }
            //Назначаем высоту в пикселях: 1 минута = 1px
            newEfir.InitHeight = newEfir.Timing / 60;

            //Используем ли Title вместо ANR
            newEfir.UseTitle = useTitle;

            return PartialView(newEfir);
        }


        


        public ActionResult ConstructEfir(EfirType curEfir, DateTime curDay, int chCode = 10, bool useTitle = false, int zapasBeg = -1)
        {

            Efir newEfir = new Efir();
            newEfir.InjectFrom(curEfir);
            
            //Ставим возраст
            if (curEfir.Age>0)
            {
                newEfir.AgeCat = curEfir.Age.ToString() + "+";
            }
            else
            {
                newEfir.AgeCat = "";
            }

            //Ставим код канала
            newEfir.ChCode = chCode;

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
            //if ((newEfir.ANR.ToUpper().Contains("ВОСКРЕСНОЕ \"ВРЕМЯ\"") || newEfir.ANR.ToUpper().Contains("НОВОСТИ") || newEfir.ANR.ToUpper().Contains("\"ВРЕМЯ\""))) 
            if (newEfir.ProducerCode=="04")
            {
                newEfir.IsNews = true;
            }

            //Выделяем другие нужные нам типы эфиров. Пока - спорт
            newEfir.IsHighlighted = false;
            if (newEfir.ProducerCode == "24")
            {
                newEfir.IsHighlighted = true;
            }
            //Получаем время окончания
            newEfir.EndTime = newEfir.Beg + TimeSpan.FromSeconds(newEfir.Timing);
            //Проверяем, укладываемся ли в текущий день
            if (newEfir.Beg.Date == curDay.Date)
            {
                newEfir.IsNextDay = false;
                newEfir.IsPrevDay = false;
            }
            else
            {
                if (newEfir.Beg.Date > curDay.Date)
                {
                    newEfir.IsNextDay = true;
                    newEfir.IsPrevDay = false;
                }
                else
                {
                    newEfir.IsNextDay = false;
                    newEfir.IsPrevDay = true;
                }
            }
            //Назначаем высоту в пикселях: 1 минута = 1px
            newEfir.InitHeight = newEfir.Timing / 60;
            
            //Используем ли Title вместо ANR
            newEfir.UseTitle = useTitle;


            //Работаем с запасом
            if (zapasBeg>=0)
            {
                newEfir.IsFromZapas = true;

                newEfir.Beg = DateTime.Now.Date + TimeSpan.FromHours(4) + TimeSpan.FromSeconds(zapasBeg);
                newEfir.IsPrevDay = false;
                newEfir.IsNextDay = false;                

            }
            else
            {
                newEfir.IsFromZapas = false;
            }

            if (newEfir.ChCode == 11)
            {
                var x = 0;
            }

            return PartialView(newEfir);
        }

        public ActionResult ConstructOrbEfir(Efir curEfir, DateTime curDay, int chCode = 10, bool useTitle = true)
        {

            Efir newEfir = new Efir();
            newEfir.InjectFrom(curEfir);


            //Ставим возраст
            if (curEfir.Age > 0)
            {
                newEfir.AgeCat = curEfir.Age.ToString() + "+";
            }
            else
            {
                newEfir.AgeCat = "";
            }

            //Ставим код канала
            newEfir.ChCode = chCode;

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
            //if ((newEfir.ANR.ToUpper().Contains("ВОСКРЕСНОЕ \"ВРЕМЯ\"") || newEfir.ANR.ToUpper().Contains("НОВОСТИ") || newEfir.ANR.ToUpper().Contains("\"ВРЕМЯ\""))) 
            if (newEfir.ProducerCode == "04")
            {
                newEfir.IsNews = true;
            }

            //Выделяем другие нужные нам типы эфиров. Пока - спорт
            newEfir.IsHighlighted = false;
            if (newEfir.ProducerCode == "24")
            {
                newEfir.IsHighlighted = true;
            }
            //Получаем время окончания
            newEfir.EndTime = newEfir.Beg + TimeSpan.FromSeconds(newEfir.Timing);
            //Проверяем, укладываемся ли в текущий день
            if (newEfir.Beg.Date == curDay.Date)
            {
                newEfir.IsNextDay = false;
                newEfir.IsPrevDay = false;
            }
            else
            {
                if (newEfir.Beg.Date > curDay.Date)
                {
                    newEfir.IsNextDay = true;
                    newEfir.IsPrevDay = false;
                }
                else
                {
                    newEfir.IsNextDay = false;
                    newEfir.IsPrevDay = true;
                }
            }
            //Назначаем высоту в пикселях: 1 минута = 1px
            newEfir.InitHeight = newEfir.Timing / 60;

            //Используем ли Title вместо ANR
            newEfir.UseTitle = useTitle;

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
            //Ставим возраст
            if (curEfir.Age > 0)
            {
                newEfir.AgeCat = curEfir.Age.ToString() + "+";
            }
            else
            {
                newEfir.AgeCat = "";
            }


                

            //Проверяем, укладываемся ли в текущий день
            if (newEfir.Beg.Date == curDay.Date)
            {
                newEfir.IsNextDay = false;
            }
            else
            {
                if (newEfir.Beg.Date > curDay.Date)
                {
                    newEfir.IsNextDay = true;
                }
                else
                {
                    newEfir.IsNextDay = false;
                    newEfir.IsPrevDay = true;
                }
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
                    if (newEfir.IsPrevDay)
                    {
                        thisNewsStart = Convert.ToInt32(newEfir.Beg.TimeOfDay.TotalSeconds) - 24 * 60 * 60;
                    }
                    else
                    {
                        thisNewsStart = Convert.ToInt32(newEfir.Beg.TimeOfDay.TotalSeconds);
                    }
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

            //Добавлено 22.03.17
            chopper1.MyStartupClass.lastNewsStart = 0;
            chopper1.MyStartupClass.totalBlockDur = 0;
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
                int limit = 0;
                while (curEfir.ANR.IndexOf("$") >= 0)
                {
                    //Постоянно глючит, т.к. если ключ не совпадает с предложенными ниже вариантами - цикл получается бесконечным.
                    limit++;
                    if (limit > 10) { break; }

                    //Размер шрифта
                    if (curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "Ш")
                    {
                        cur_key = curEfir.ANR.Substring(curEfir.ANR.IndexOf("$"), 3);
                        curEfir.FontSize = Convert.ToInt32(cur_key.Right(1)) + 1;
                        curEfir.ANR = curEfir.ANR.Replace(cur_key, "");
                    }
                    //Загадочный ключ
                    if (curEfir.ANR.IndexOf("$") >= 0 & (curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "X" || curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "Х"))
                    {
                        cur_key = curEfir.ANR.Substring(curEfir.ANR.IndexOf("$"), 2);
                        curEfir.Reserv = true;
                        curEfir.ANR = curEfir.ANR.Replace(cur_key, "");
                    }
                    //Заливка
                    if (curEfir.ANR.IndexOf("$") >= 0 & (curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "C" || curEfir.ANR.Substring(curEfir.ANR.IndexOf("$") + 1, 1) == "С"))
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
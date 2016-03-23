using chopper1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace chopper1.Controllers
{
    public class advSearchController : Controller
    {

        public advSearch advSPanel = new advSearch();
        // GET: advSearch
        public ActionResult advSearchPanel(string title = "", DateTime? dateMin = null, DateTime? dateMax = null, DateTime? timeStartMin = null, DateTime? timeStartMax = null, DateTime? timingMin = null, DateTime? timingMax = null, bool monday = true, bool tuesday = true, bool wednesday = true, bool thursday = true, bool friday = true, bool saturday = true, bool sunday = true, List<string> Producers = null, bool BitOriginal = true, bool BitRepetition = true)
        {
            if (title == "")
            {
                if (Request.Form["catSearchDb"] != null)
                {
                    title = Request.Form["catSearchTb"].ToString();
                }
                else
                {
                    title = "";
                }
            }                        
            advSPanel.Title = title;
            if (dateMin == null)
            {
                advSPanel.DateMin = DateTime.Parse("01/01/2005");
            }
            else
            {
                advSPanel.DateMin = Convert.ToDateTime(dateMin);
            }
            if (dateMax == null)
            {
                advSPanel.DateMax = DateTime.Now;
            }
            else
            {
                advSPanel.DateMax = Convert.ToDateTime(dateMax);
            }
            if (timeStartMin == null)
            {
                advSPanel.TimeStartMin = DateTime.Parse("00:00:00");
            }
            else
            {
                advSPanel.TimeStartMin = Convert.ToDateTime(timeStartMin);
            }
            if (timeStartMax == null)
            {
                advSPanel.TimeStartMax = DateTime.Parse("23:59:00");
            }
            else
            {
                advSPanel.TimeStartMax = Convert.ToDateTime(timeStartMax);
            }
            if (timingMin == null)
            {
                advSPanel.TimingMin = DateTime.Parse("00:01:00");
            }
            else
            {
                advSPanel.TimingMin = Convert.ToDateTime(timingMin);
            }
            if (timingMax == null)
            {
                advSPanel.TimingMax = DateTime.Parse("10:00:00");
            }
            else
            {
                advSPanel.TimingMax = Convert.ToDateTime(timingMax);
            }
            advSPanel.Monday = monday;
            advSPanel.Tuesday= tuesday;
            advSPanel.Wednesday = wednesday;
            advSPanel.Thursday = thursday;
            advSPanel.Friday = friday;
            advSPanel.Saturday = saturday;
            advSPanel.Sunday = sunday;

            advSPanel.BitRepetition = BitRepetition;
            advSPanel.BitOriginal = BitOriginal;

            ViewBag.stitle = advSPanel.Title;
            ViewData["prodList"] = getProducersList();
            ViewData["Producers"] = advSPanel.Producers;
            return View(advSPanel);
        }

        public ActionResult PerformAdvSearch(string Title = "", DateTime? DateMin = null, DateTime? DateMax = null, DateTime? TimeStartMin = null, DateTime? TimeStartMax = null, DateTime? TimingMin = null, DateTime? TimingMax = null, bool Monday = true, bool Tuesday = true, bool Wednesday = true, bool Thursday = true, bool Friday = true, bool Saturday = true, bool Sunday = true, List<string> Producers = null, bool BitOriginal = true, bool BitRepetition = true)
        {
            

            planCatDb pDb = new planCatDb();
            pDb.Open(MyStartupClass.curCatConnection);
            var prodDt = pDb.advSearch(Title,DateMin,DateMax,TimeStartMin,TimeStartMax,TimingMin,TimingMax,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,Producers, BitOriginal, BitRepetition);
            pDb.Close();
            ViewBag.stitle = Title;
            return View(prodDt);
        }

        private List<string> getProducersList()
        {

            planCatDb pDb = new planCatDb();
            pDb.Open(MyStartupClass.curCatConnection);
            var prodDt = pDb.getProdList();
            DataRow[] rows = prodDt.Select();
            var prodList = Array.ConvertAll(rows, row => row[0].ToString());
            pDb.Close();
            return prodList.ToList();
        }


    }
}
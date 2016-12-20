using chopper1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;
using System.ComponentModel;
using System.Diagnostics;

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

        public ActionResult PerformAdvSearch0(string Title = "", DateTime? DateMin = null, DateTime? DateMax = null, DateTime? TimeStartMin = null, DateTime? TimeStartMax = null, DateTime? TimingMin = null, DateTime? TimingMax = null, bool Monday = true, bool Tuesday = true, bool Wednesday = true, bool Thursday = true, bool Friday = true, bool Saturday = true, bool Sunday = true, List<string> Producers = null, bool BitOriginal = true, bool BitRepetition = true)
        {
            

            planCatDb pDb = new planCatDb();
            pDb.Open(MyStartupClass.curCatConnection);
            var prodDt = pDb.advSearch(Title,DateMin,DateMax,TimeStartMin,TimeStartMax,TimingMin,TimingMax,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,Producers, BitOriginal, BitRepetition);
            pDb.Close();
            ViewBag.stitle = Title;
            return View(prodDt);
        }

        public ActionResult PerformAdvSearch(string Title = "", DateTime? DateMin = null, DateTime? DateMax = null, DateTime? TimeStartMin = null, DateTime? TimeStartMax = null, DateTime? TimingMin = null, DateTime? TimingMax = null, bool Monday = true, bool Tuesday = true, bool Wednesday = true, bool Thursday = true, bool Friday = true, bool Saturday = true, bool Sunday = true, List<string> Producers = null, bool BitOriginal = true, bool BitRepetition = true)
        {

            WebСервис1 curWc = MyStartupClass.wc;
            PokazType[] resultArray = curWc.GetPokazs(DateMin, DateMax, Title);
            
            List<PokazType> filteredResultList = new List<PokazType>();
            DateTime n = Convert.ToDateTime(TimeStartMin);                        

            if (DateMin == null)
            {
                DateMin = DateTime.Parse("2005-01-01 00:00:00");
                TimeStartMin = DateMin;
            }
            if (DateMax == null)
            {
                DateMax = DateTime.Now;
                TimeStartMax = DateTime.Now.Date+TimeSpan.FromMinutes(23*60+59);
            }
            if (TimingMin == null)
            {
                TimingMin = DateTime.Now.Date + TimeSpan.FromMinutes(00 * 60 + 01);
            }
            if (TimingMax == null)
            {
                TimingMax = DateTime.Now.Date + TimeSpan.FromMinutes(10 * 60 + 00);
            }

            foreach (PokazType p in resultArray)
            {
                if (p.Beg.Hour * 60 + p.Beg.Minute >= Convert.ToDateTime(TimeStartMin).Hour * 60 + Convert.ToDateTime(TimeStartMin).Minute & p.Beg.Hour * 60 + p.Beg.Minute <= Convert.ToDateTime(TimeStartMax).Hour * 60 + Convert.ToDateTime(TimeStartMax).Minute)
                {
                    if (p.Timing >= Convert.ToDateTime(TimingMin).Hour * 60 * 60 + Convert.ToDateTime(TimingMin).Minute * 60 & p.Timing <= Convert.ToDateTime(TimingMax).Hour * 60 * 60 + Convert.ToDateTime(TimingMax).Minute * 60)
                    {
                        if ((p.TVData.DayOfWeek==DayOfWeek.Monday & Monday) | (p.TVData.DayOfWeek==DayOfWeek.Tuesday & Tuesday) |(p.TVData.DayOfWeek==DayOfWeek.Wednesday & Wednesday) |(p.TVData.DayOfWeek==DayOfWeek.Thursday & Thursday) |(p.TVData.DayOfWeek==DayOfWeek.Friday & Friday) |(p.TVData.DayOfWeek==DayOfWeek.Saturday & Saturday) |(p.TVData.DayOfWeek==DayOfWeek.Sunday & Sunday))
                        {
                            //if ((BitOriginal & BitRepetition) | (BitOriginal&(p.Premiere | p.RepeatFrom != null)) | (BitRepetition&!BitOriginal&!p.Premiere))
                            if ((BitOriginal & p.RepeatFrom.Year == DateTime.Parse("01.01.0001").Year) | (BitRepetition & p.RepeatFrom.Year != DateTime.Parse("01.01.0001").Year))
                            {
                                if (Producers != null)
                                {
                                    foreach (string prod in Producers)
                                    {
                                        if (prod.Left(prod.IndexOf(" "))==p.ProducerCode.ToString())
                                        {
                                            filteredResultList.Add(p);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    filteredResultList.Add(p);
                                }
                            }
                        }
                    }
                }
            }

           
            DataTable prodDt = ConvertToDatatable(filteredResultList);
            
        
                        
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

        private static DataTable ConvertToDatatable<T>(List<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
                
            }
            //object[] values = new object[props.Count];
            object[] values = new object[table.Columns.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }



            table.Columns["Timing"].SetOrdinal(0);
            table.Columns["BriefTitle"].SetOrdinal(1);
            table.Columns["RepeatFrom"].SetOrdinal(2);
            table.Columns["TVData"].SetOrdinal(3);
            table.Columns["WeekDay"].SetOrdinal(4);
            table.Columns["Beg"].SetOrdinal(5);
            table.Columns["DSTI"].SetOrdinal(6);
            table.Columns["DM"].SetOrdinal(7);
            table.Columns["DR"].SetOrdinal(8);
            table.Columns["ProducerCode"].SetOrdinal(9);
            table.Columns["SellerCode"].SetOrdinal(10);

            table.DefaultView.Sort = "TVData, Beg";
            table = table.DefaultView.ToTable();

            return table;
        }


    }
}
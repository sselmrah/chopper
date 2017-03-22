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
            PokazType curPokaz = new PokazType();
            DateTime dtMin = DateTime.Parse("01-01-2007"); ;
            DateTime dtMax = DateTime.Now;
            string ratTitle = '%' + Title + '%';
            RatEfirType[] ratingsArray = curWc.GetRatEfirs2(10, dtMin, dtMax, ratTitle); 
            EfirType[] efirsArray;


            List<PokazType> pokazList = new List<PokazType>();
            if (DateMin != null)
            {
                dtMin = (DateTime)DateMin;
            }
            if (DateMax != null)
            {
                dtMin = (DateTime)DateMax;
            }
            
            if (resultArray.Length == 0)
            {
                //Eng to Rus switch if necessary
                resultArray = curWc.GetPokazs(DateMin, DateMax, Title.EngToRus());
                if (resultArray.Length>0)
                {
                    Title = Title.EngToRus();
                    ratTitle = '%' + Title + '%';
                }
                /*
                if (resultArray.Length == 0)
                {
                    ratingsArray = curWc.GetRatEfirs2(10, dtMin, dtMax, ratTitle);

                   
                    foreach (RatEfirType r in ratingsArray)
                    {
                        curPokaz.Beg = r.Beg;
                        curPokaz.BriefTitle = r.Title;
                        curPokaz.Title = r.Title;
                        curPokaz.ChannelCode = 10;
                        curPokaz.DM = (float)r.DM;
                        curPokaz.RM = (float)r.RM;
                        curPokaz.DR = (float)r.DR;
                        curPokaz.RR = (float)r.RR;
                        curPokaz.DSTI = (float)r.DSTI;
                        curPokaz.ProducerCode = 0;
                        curPokaz.SellerCode = 0;
                        curPokaz.Timing = r.Timing;
                        curPokaz.TVData = r.TVDate;

                        pokazList.Add(curPokaz);
                    }
                    resultArray = pokazList.ToArray();
                }
                 */ 
            }

            //Ищем в рейтингах
            pokazList = resultArray.ToList();
            if (resultArray.Length > 0)
            {
                dtMin = pokazList[pokazList.Count() - 1].TVData.Date + TimeSpan.FromDays(1);
            }
            ratingsArray = curWc.GetRatEfirs2(10, dtMin, dtMax, ratTitle);           
            foreach (RatEfirType r in ratingsArray)
            {
                curPokaz.Beg = r.Beg;
                curPokaz.BriefTitle = r.Title;
                curPokaz.Title = r.Title;
                curPokaz.ChannelCode = 10;
                curPokaz.DM = (float)r.DM;
                curPokaz.RM = (float)r.RM;
                curPokaz.DR = (float)r.DR;
                curPokaz.RR = (float)r.RR;
                curPokaz.DSTI = (float)r.DSTI;
                curPokaz.ProducerCode = 0;
                curPokaz.SellerCode = 0;
                curPokaz.Timing = r.Timing;
                curPokaz.TVData = r.TVDate;
                curPokaz.WeekDay = (int)r.TVDate.DayOfWeek;
                if (curPokaz.WeekDay == 0) curPokaz.WeekDay = 7;
                pokazList.Add(curPokaz);
            }
            resultArray = pokazList.ToArray();

            //Ищем в эфирах
           
            if (resultArray.Length > 0)
            {
                dtMin = pokazList[pokazList.Count() - 1].TVData.Date + TimeSpan.FromDays(1);
            }
            dtMax += TimeSpan.FromDays(365);
            efirsArray = curWc.GetEfirs2(10,dtMin, dtMax, ratTitle);
            foreach (EfirType ef in efirsArray)
            {
                PokazType curPokaz2 = new PokazType();
                curPokaz2.Beg = ef.Beg;
                curPokaz2.BriefTitle = ef.Title;
                curPokaz2.Title = ef.Title;
                curPokaz2.ChannelCode = 10;
                curPokaz2.DM = 0;
                curPokaz2.RM = 0;
                curPokaz2.DR = 0;
                curPokaz2.RR = 0;
                curPokaz2.DSTI = 0;
                curPokaz2.ProducerCode = Convert.ToInt32(ef.ProducerCode);
                curPokaz2.SellerCode = Convert.ToInt32(ef.SellerCode);
                curPokaz2.Timing = MyStartupClass.getTimingWoITC(ef);                
                curPokaz2.TVData = ef.Beg.Date;
                curPokaz2.WeekDay = (int)ef.Beg.DayOfWeek;
                if (curPokaz2.WeekDay == 0) curPokaz2.WeekDay = 7;
                pokazList.Add(curPokaz2);
            }
            resultArray = pokazList.ToArray();
            


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
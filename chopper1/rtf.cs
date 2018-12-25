using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;
using chopper1.Models;
using System.Web.Mvc;

using System.Text;
using System.Globalization;
using Omu.ValueInjecter;
using System.Reflection;
using System.IO;
using System.Diagnostics;



namespace chopper1
{
    public class rtf
    {
        public static WebСервис1 wc = MyStartupClass.wc;
        public static CultureInfo russian = new CultureInfo("ru-RU"); 

        public static FileStreamResult printReport(string dayVariantList = "", string repType = "broadcast", string pointer = "2016-07-06", bool pdf = false, bool word = true, bool print = false)
        {

            string curText = "";

            List<string> rtfList = new List<string>();

            string path = "~/";
            string fileName = repType + dayVariantList.Left(10).Replace(".", "") + "_" + DateTime.Now.ToString("HHmmss") + ".rtf";
            string fullPath = path + fileName;

            rtfList.Add(rtfHead());

            //Недельная раскладка
            if (repType == "raskladka")
            {
                rtfList.AddRange(printRaskladka(dayVariantList));
            }

            //Соколовский вариант
            if (repType == "broadcast")
            {
                rtfList.AddRange(printBroadcast(dayVariantList));
            }
            //Соколовский вариант11
            if (repType == "broadcast11")
            {
                rtfList.AddRange(printBroadcast11(dayVariantList));
            }
            //Сводка
            if (repType == "svodka")
            {
                rtfList.AddRange(printSvodka(dayVariantList));
            }
            //Орбиты
            if (repType == "orbity")
            {
                rtfList.AddRange(printOrbity(dayVariantList));
            }
            //Столбы
            if (repType == "stolby")
            {
                rtfList.AddRange(printStolby(dayVariantList));
            }
            //Рейтинги
            if (repType == "ratings")
            {
                rtfList.AddRange(printRatings(dayVariantList));
            }

            //Timestamp
            curText = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm");
            rtfList.Add(rtfProg(FontSize: 8, Text: curText, XPos: 14500, YPos: 200, XSize: 1700, YSize: 200, Fill: false, Italic: true, Line: false));
            //Footer
            rtfList.Add(rtfFoot());
            string[] rtfArray = rtfList.ToArray();
            System.IO.File.WriteAllLines(HttpContext.Current.Server.MapPath(fullPath), rtfArray, Encoding.GetEncoding(1251));

            string mimeType = "application/rtf";

            FileStreamResult result = new FileStreamResult(new FileStream(HttpContext.Current.Server.MapPath(fullPath), FileMode.Open), mimeType);
            result.FileDownloadName = fileName;

            return result;
        }

        private static List<string> printSvodka(string dayVariantList = "")
        {
            double twipsCoef = 4.233;
            int xPos = 0;
            int xSize = 2900;
            int yPos = 650;
            int begTwips = 0;
            int timingTwips = 0;
            bool fill = false;
            string curText = "";

            List<string> rtfList = new List<string>();
            List<Day> newDays = new List<Day>();
            newDays = getSvodkaDaysFromHtml(dayVariantList);

            string dow = newDays[0].TVDate.ToString("dddd", russian).ToUpper();
            if (dow == "СРЕДА") dow = "СРЕДУ";
            if (dow == "ПЯТНИЦА") dow = "ПЯТНИЦУ";
            if (dow == "СУББОТА") dow = "СУББОТУ";

            //Header
            curText = "ЭФИРНАЯ СВОДКА НА " + dow + ", " + newDays[0].TVDate.ToString("dd/MM/yyyy");
            rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line: false, XPos: 190, YPos: 80, XSize: 16000, YSize: 230));

            for (int i = 0; i < 5; i++)
            {
                xPos = 190 + i * xSize + i * 200;

                //Channel name
                if (i == 0)
                {
                    curText = "Первый канал";
                }
                else
                {
                    curText = "Орбита " + (5 - i).ToString();
                }
                if (curText == "Орбита 4") curText += " (HD и SD)";
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 400, XSize: xSize, YSize: 250));

                //Orbit timeshift
                int orbShift = 0;
                orbShift = i * 2;

                //Left timescale
                rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: 0, left: true, orbShift: orbShift, xSize: xSize));
                if (newDays[i].Efirs != null)
                {
                    foreach (EfirType ef in newDays[i].Efirs)
                    {
                        curText = "";
                        if (ef.Beg.Date == newDays[i].TVDate)
                        {
                            begTwips = Convert.ToInt32(((ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                        }
                        else
                        {
                            if (ef.Beg.Date > newDays[i].TVDate)
                            {
                                begTwips = Convert.ToInt32((24 * 60 * 60 + (ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                            }
                            else
                            {
                                begTwips = Convert.ToInt32(((ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60 - 24 * 60 * 60) / twipsCoef);
                            }
                        }
                        begTwips -= 3100;
                        timingTwips = Convert.ToInt32(ef.Timing / twipsCoef);
                        Efir tempEfir = MyStartupClass.getRTA(ef.Timing, ef.ITC);

                        //Timing
                        /*
                        string strTiming = TimeSpan.FromSeconds(ef.Timing).Hours + ":" + TimeSpan.FromSeconds(ef.Timing).Minutes + ":" + TimeSpan.FromSeconds(ef.Timing).Seconds;
                        if (strTiming.Right(2) == "00") strTiming.Substring(0, strTiming.Length - 3);
                        if (strTiming.Left(2) == "0:") strTiming.Substring(2);
                        */
                        string strTiming = getTimingString(ef.Timing);
                        //Text
                        curText += ef.Beg.ToString("HH:mm") + " - " + (ef.Beg + TimeSpan.FromSeconds(ef.Timing)).ToString("HH:mm") + " (" + strTiming + ")";
                        if (ef.Timing > 10 * 60)
                        {
                            curText += "\\line";
                        }
                        curText += ef.Title.ToUpper();

                        //Fill
                        if (ef.ProducerCode == "04" | ef.ProducerCode == "24" | (ef.ANR.ToLower().Contains("толстой") & ef.ANR.ToLower().Contains("воскресенье")))
                        {
                            fill = true;
                        }
                        else
                        {
                            fill = false;
                        }

                        //Infostring
                        curText += "   [" + tempEfir.getInfoString() + "] (" + ef.ProducerCode + ef.SellerCode + ")";

                        rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos, YPos: begTwips, XSize: xSize, YSize: timingTwips, Fill: fill));
                        //AgeCat
                        if (ef.Age > 0)
                        {
                            curText = ef.Age.ToString() + "+";
                            rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos + xSize - 240, YPos: begTwips, XSize: 240, YSize: 150, Fill: false, Bold: true, Italic: false));
                        }
                    }
                    if (i == 4)
                    {
                        //Rightmost timescale
                        xPos = 190 + 5 * xSize + 5 * 200;
                        orbShift = 0;
                        rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: 0, left: false, orbShift: orbShift, xSize: xSize));
                    }
                }

            }
            return rtfList;
        }

        private static List<string> printOrbity(string dayVariantList = "")
        {
            string curText = "";
            List<string> rtfList = new List<string>();
            Week curWeek = new Week();
            TVWeekType curTvWeek = new TVWeekType();
            int[] array_channel_codes = new int[1];
            array_channel_codes[0] = 10;

            //Print params
            int xSize = 15400;
            int xPos = 190;
            int dayNum = -1;
            double twipsCoef = 4.233;
            int yPosShift = 650;
            int begTwips = 0;
            int timingTwips = 0;
            bool fill = false;
            bool bold = false;
            bool italic = false;


            //Получаем список дней
            List<Day> newDayList = getDaysFromHtml(dayVariantList);
            List<Day> mainDayList = new List<Day>();
            foreach (Day d in newDayList)
            {
                if (d.KanalKod == 10)
                {
                    d.OrbEfirs = chopper1.Controllers.DayController.getOrbEfirsList(d.TVDate, d.KanalKod, d.VariantKod);
                    mainDayList.Add(d);
                }

            }
            //Header
            curText = "ПРОГРАММА ПЕРЕДАЧ С " + mainDayList[0].TVDate.ToString("dd/MM/yyyy") + " ПО " + mainDayList[mainDayList.Count() - 1].TVDate.ToString("dd/MM/yyyy");
            rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line: false, XPos: 190, YPos: 40, XSize: 16000, YSize: 230));

            //Left timescale
            rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: yPosShift, left: true, orbShift: 0, xSize: xSize));

            xSize = 15400 / 4;
            foreach (Day d in mainDayList)
            {
                if (dayNum == 3)
                {
                    dayNum = 0;
                    rtfList.Add("\\pagebb\\par");
                }
                else
                {
                    dayNum++;
                }
                xPos = 190 + dayNum * xSize;
                //Captions
                curText = d.DoWRus + ", " + d.TVDate.ToString("dd.MM.yyyy");
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 400, XSize: xSize, YSize: 250));
                curText = "I";
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 650, XSize: xSize / 5, YSize: 250));
                //rtfList.Add(rtfLine(gor: false, xPos: xPos, yPos: 1750, len: Convert.ToInt32(24*60*60/twipsCoef), punktir: true, width: 1));
                curText = "О4";
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos + (xSize / 5) * 1, YPos: 650, XSize: xSize / 5, YSize: 250));
                rtfList.Add(rtfLine(gor: false, xPos: xPos + (xSize / 5) * 1, yPos: 1750, len: Convert.ToInt32(24 * 60 * 60 / twipsCoef), punktir: true, width: 4));
                curText = "О3";
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos + (xSize / 5) * 2, YPos: 650, XSize: xSize / 5, YSize: 250));
                rtfList.Add(rtfLine(gor: false, xPos: xPos + (xSize / 5) * 2, yPos: 1750, len: Convert.ToInt32(24 * 60 * 60 / twipsCoef), punktir: true, width: 4));
                curText = "О2";
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos + (xSize / 5) * 3, YPos: 650, XSize: xSize / 5, YSize: 250));
                rtfList.Add(rtfLine(gor: false, xPos: xPos + (xSize / 5) * 3, yPos: 1750, len: Convert.ToInt32(24 * 60 * 60 / twipsCoef), punktir: true, width: 4));
                curText = "О1";
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos + (xSize / 5) * 4, YPos: 650, XSize: xSize / 5, YSize: 250));
                rtfList.Add(rtfLine(gor: false, xPos: xPos + (xSize / 5) * 4, yPos: 1750, len: Convert.ToInt32(24 * 60 * 60 / twipsCoef), punktir: true, width: 4));

                //FullCap
                curText = d.FullCap;
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 900, XSize: xSize, YSize: 850));
                if (d.OrbEfirs != null)
                {
                    foreach (Efir ef in d.OrbEfirs)
                    {
                        xPos = 190 + dayNum * xSize;
                        int orbWidth;
                        orbWidth = xSize / 5;
                        //New Width
                        int widthInt = 0;
                        if (ef.OrbCh1) widthInt += 1;
                        if (ef.Orb1) widthInt += 1;
                        if (ef.Orb2) widthInt += 1;
                        if (ef.Orb3) widthInt += 1;
                        if (ef.Orb4) widthInt += 1;
                        //Horizontal positioning                        
                        if (ef.OrbCh1)
                        {
                            if (ef.Orb4)
                            {
                                orbWidth += xSize / 5;
                            }
                            if (ef.Orb3)
                            {
                                orbWidth += xSize / 5;
                            }
                            if (ef.Orb2)
                            {
                                orbWidth += xSize / 5;
                            }
                            if (ef.Orb1)
                            {
                                orbWidth += xSize / 5;
                            }
                        }
                        else
                        {
                            if (ef.Orb4)
                            {
                                xPos += (xSize / 5);
                                if (ef.Orb3)
                                {
                                    orbWidth += xSize / 5;
                                }
                                if (ef.Orb2)
                                {
                                    orbWidth += xSize / 5;
                                }
                                if (ef.Orb1)
                                {
                                    orbWidth += xSize / 5;
                                }
                            }
                            else
                            {
                                if (ef.Orb3)
                                {
                                    xPos += ((xSize / 5) * 2);
                                    if (ef.Orb2)
                                    {
                                        orbWidth += xSize / 5;
                                    }
                                    if (ef.Orb1)
                                    {
                                        orbWidth += xSize / 5;
                                    }
                                }
                                else
                                {
                                    if (ef.Orb2)
                                    {
                                        xPos += ((xSize / 5) * 3);
                                        if (ef.Orb1)
                                        {
                                            orbWidth += xSize / 5;
                                        }
                                    }
                                    else
                                    {
                                        if (ef.Orb1)
                                        {
                                            xPos += ((xSize / 5) * 4);
                                        }
                                    }
                                }
                            }
                        }


                        curText = "";
                        if (ef.Beg.Date == d.TVDate)
                        {
                            begTwips = Convert.ToInt32((ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                        }
                        else
                        {
                            if (ef.Beg.Date > d.TVDate)
                            {
                                begTwips = Convert.ToInt32((24 * 60 * 60 + ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                            }
                            else
                            {
                                begTwips = Convert.ToInt32((ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60 - 24 * 60 * 60) / twipsCoef);
                            }
                        }




                        begTwips -= 3100 - yPosShift;
                        timingTwips = Convert.ToInt32(ef.Timing / twipsCoef);
                        Efir tempEfir = MyStartupClass.getRTA(ef.Timing, ef.ITC);

                        //Timing
                        //string strTiming = TimeSpan.FromSeconds(ef.Timing).Hours + ":" + TimeSpan.FromSeconds(ef.Timing).Minutes + ":" + TimeSpan.FromSeconds(ef.Timing).Seconds;                        
                        //if (strTiming.Right(2) == "00") strTiming.Substring(0, strTiming.Length - 3);
                        //if (strTiming.Left(2) == "0:") strTiming.Substring(2);
                        string strTiming = getTimingString(ef.Timing);
                        //Text
                        /*
                        curText += ef.Beg.ToString("HH:mm") + " - " + (ef.Beg + TimeSpan.FromSeconds(ef.Timing)).ToString("HH:mm") + " (" + strTiming + ")";
                        curText += "\\line";
                         */
                        curText += ef.Title.ToUpper();





                        //Fill
                        if (ef.ProducerCode == "04" | ef.ProducerCode == "24" | (ef.ANR.ToLower().Contains("толстой") & ef.ANR.ToLower().Contains("воскресенье")))
                        {
                            fill = true;
                        }
                        else
                        {
                            fill = false;
                        }
                        /*
                        string cur_key;
                        if (ef.ANR.Contains("$Ш") || ef.ANR.Contains("$Х") || ef.ANR.Contains("$X") || ef.ANR.Contains("$C") || ef.ANR.Contains("$С") || ef.ANR.Contains("$Ц"))
                        {
                            while (ef.ANR.IndexOf("$") >= 0)
                            {
                                //Размер шрифта
                                if (ef.ANR.Substring(ef.ANR.IndexOf("$") + 1, 1) == "Ш")
                                {
                                    cur_key = ef.ANR.Substring(ef.ANR.IndexOf("$"), 3);
                                    curText = cur_key + curText;
                                    //FontSize = Convert.ToInt32(cur_key.Right(1))-1;
                                    //FontSize = Convert.ToInt32(cur_key.Right(1)) / 2;
                                    ef.ANR = ef.ANR.Replace(cur_key, "");
                                }
                                //Загадочный ключ
                                if (ef.ANR.IndexOf("$") >= 0 & (ef.ANR.Substring(ef.ANR.IndexOf("$") + 1, 1) == "X" || ef.ANR.Substring(ef.ANR.IndexOf("$") + 1, 1) == "Х"))
                                {
                                    cur_key = ef.ANR.Substring(ef.ANR.IndexOf("$"), 2);
                                    curText = cur_key + curText;
                                    //curEfir.Reserv = true;
                                    ef.ANR = ef.ANR.Replace(cur_key, "");
                                }
                                //Заливка
                                if (ef.ANR.IndexOf("$") >= 0 & (ef.ANR.Substring(ef.ANR.IndexOf("$") + 1, 1) == "C" || ef.ANR.Substring(ef.ANR.IndexOf("$") + 1, 1) == "С"))
                                {

                                    cur_key = ef.ANR.Substring(ef.ANR.IndexOf("$"), 4);
                                    curText = cur_key + curText;
                                    //Fill = true;
                                    ef.ANR = ef.ANR.Replace(cur_key, "");
                                }

                            }
                        }
                        */



                        if (ef.Title.ToLower().Contains("\\b") | ef.ANR.ToLower().Contains("\\b"))
                        {
                            ef.Title = ef.Title.Replace("\\b0", "");
                            ef.Title = ef.Title.Replace("\\b1", "");
                            ef.Title = ef.Title.Replace("\\b", "");
                            bold = true;
                        }
                        else
                        {
                            bold = false;
                        }
                        if (ef.Title.ToLower().Contains("\\i") | ef.ANR.ToLower().Contains("\\i"))
                        {
                            ef.Title = ef.Title.Replace("\\i", "");
                            italic = true;
                        }
                        else
                        {
                            italic = false;
                        }






                        //Infostring
                        curText += "   [" + tempEfir.getInfoString() + "] (" + ef.ProducerCode + ef.SellerCode + ")";

                        rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos, YPos: begTwips, XSize: Convert.ToInt32(orbWidth), YSize: timingTwips, Fill: fill, Bold: bold, Italic: italic));

                        //AgeCat
                        if (ef.Age > 0)
                        {
                            curText = ef.Age.ToString() + "+";
                            rtfList.Add(rtfProg(FontSize: 4, Text: curText, XPos: xPos + orbWidth - 180, YPos: begTwips, XSize: 180, YSize: 100, Fill: false, Bold: true, Italic: false));
                        }


                    }

                }
            }
            //Rightmost timescale
            for (int i = 0; i < 5; i++)
            {
                xPos = 190 + 3 * xSize + 200 + (xSize / 5) * i;
                rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: yPosShift, left: false, orbShift: i * 2, xSize: xSize));
            }
            //Timescale Labels
            xPos = 190 + 3 * xSize + 200;
            curText = "#Время местное";
            rtfList.Add(rtfProg(Text: curText, FontSize: 7, Bold: false, Line: false, XPos: xPos - 120, YPos: 400 + 600, XSize: (xSize / 5), YSize: 500));
            xPos = 190 + 3 * xSize + 200;
            curText = "Время московское";
            rtfList.Add(rtfProg(Text: curText, FontSize: 7, Bold: false, Line: false, XPos: xPos, YPos: 650 + 600, XSize: (xSize / 5) * 4, YSize: 250));

            return rtfList;

        }

        private static List<string> printStolby(string dayVariantList = "")
        {
            double twipsCoef = 4.233;
            int xPos = 0;
            int xSize = 2900;
            int yPos = 650;
            int begTwips = 0;
            int timingTwips = 0;
            bool fill = false;
            string curText = "";

            List<string> rtfList = new List<string>();
            List<Day> newDays = new List<Day>();
            newDays = getDaysFromHtml(dayVariantList);

            string dow = newDays[0].TVDate.ToString("dddd", russian).ToUpper();
            
            //Header
            //curText = "ПРОГРАММА ПЕРЕДАЧ ПЕРВОГО КАНАЛА НА " + dow + ", " + newDays[0].TVDate.ToString("dd/MM/yyyy");
            //rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line: false, XPos: 190, YPos: 80, XSize: 16000, YSize: 230));

            
            
            

            for (int day = 0; day < newDays.Count/5; day++)
            {
                string fullDateStr = newDays[day * 5].TVDate.ToString("dddd, dd MMMM yyyy", russian) + " года";
                fullDateStr = fullDateStr.Substring(0, 1).ToUpper() + fullDateStr.Substring(1);
                rtfList.Add("\\sectd\\cols1\\pard\\qc\\f1\\fs24\\b" + fullDateStr + "\\b0\\par");
                rtfList.Add("\\sect\\sbknone\\cols5\\colsx10\\linebetcol");
                
                for (int i = 0; i < 5; i++)
                {                    
                    //Channel name
                    if (i == 0)
                    {
                        curText = "Первый канал (Европейская часть РФ)";
                    }
                    else
                    {
                        curText = "Первый канал (Орбита " + (i).ToString()+")";
                    }
                    if (curText.Contains("Орбита 3")) curText += " (HD и SD)";

                    rtfList.Add("\\pard\\qc\\f1\\fs16\\b"+curText+"\\par\\b0\\ql\n");                    
                    
                    if (newDays[day*5+i].Efirs != null)
                    {
                        foreach (EfirType ef in newDays[day*5+i].Efirs)
                        {
                            if (ef.ProducerCode == "24")
                            {
                                rtfList.Add("\\trowd\\trpat3\\trgapf10\n");
                            }
                            else
                            {
                                rtfList.Add("\\trowd\\trpat0\\trgapf10\n");
                            }
                            curText = "";                           
                            Efir tempEfir = MyStartupClass.getRTA(ef.Timing, ef.ITC);                                                   
                            curText = " [" + tempEfir.getInfoString(1) + "]";
                            string titleAge = "";
                            if (ef.Age>0)
                            {
                                titleAge = ef.Title + " \\b (" + ef.Age.ToString() + "+) \\b0";
                            }
                            else
                            {
                                titleAge = ef.Title;
                            }
                            if (ef.ProducerCode == "04" | ef.ProducerCode == "24")
                            {
                                rtfList.Add("\\b\\cellx380\\f1\\fs14\\cellx2860\\cellx3231 \n" + ef.Beg.ToString("HH.mm") + "\\intbl\\cell\n" + titleAge + curText + "\\b0\\intbl\\cell\\fs12\n" + ef.ProducerCode + ef.SellerCode + "\\intbl\\cell\\f1\\fs12");
                            }
                            else
                            {
                                rtfList.Add("\\cellx380\\f1\\fs14\\cellx2860\\cellx3231 \n" + ef.Beg.ToString("HH.mm") + "\\intbl\\cell\n" + titleAge + curText + "\\intbl\\cell\\fs12\n" + ef.ProducerCode + ef.SellerCode + "\\intbl\\cell\\f1\\fs12");
                            }
                            rtfList.Add("\\row");                            
                        }                        
                    }
                    if (i < 4)
                    {
                        rtfList.Add("\\column");
                    }
                }                
                rtfList.Add("\\pard\\sect\\sbkpage");
            }
            return rtfList;
        }

        private static List<string> printBroadcast(string dayVariantList = "")
        {
            double twipsCoef = 4.233;
            int xPos = 0;
            int xSize = 2900;
            int yPos = 650;
            int begTwips = 0;
            int timingTwips = 0;
            bool fill = false;
            string curText = "";

            List<string> rtfList = new List<string>();
            List<Day> newDays = new List<Day>();
            newDays = getDaysFromHtml(dayVariantList);

            string dow = newDays[0].TVDate.ToString("dddd", russian).ToUpper();
            if (dow == "СРЕДА") dow = "СРЕДУ";
            if (dow == "ПЯТНИЦА") dow = "ПЯТНИЦУ";
            if (dow == "СУББОТА") dow = "СУББОТУ";

            //Header
            curText = "ПРОГРАММА ПЕРЕДАЧ ПЕРВОГО КАНАЛА НА " + dow + ", " + newDays[0].TVDate.ToString("dd/MM/yyyy");
            rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line: false, XPos: 190, YPos: 80, XSize: 16000, YSize: 230));

            for (int i = 0; i < 5; i++)
            {
                xPos = 190 + i * xSize + i * 200;

                //Channel name
                if (i == 0)
                {
                    curText = "Первый канал";
                }
                else
                {
                    curText = "Орбита " + (i).ToString();
                }
                switch (i)
                {
                    case 0:
                        curText = "Первый канал";
                        break;
                    case 4:
                        curText = "ПК+9";
                        break;
                    default:
                        curText = "ПК+" + i*2;
                        break;
                }
                


                //if (curText == "Орбита 4") curText += " (HD и SD)";
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 400, XSize: xSize, YSize: 250));

                //Orbit timeshift
                int orbShift = 0;
                orbShift = i * 2;
                if (i==4) {orbShift++; };

                //Left timescale
                rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: 0, left: true, orbShift: orbShift, xSize: xSize));
                if (newDays[i].Efirs != null)
                {
                    foreach (EfirType ef in newDays[i].Efirs)
                    {
                        curText = "";
                        if (ef.Beg.Date == newDays[i].TVDate)
                        {
                            begTwips = Convert.ToInt32(((ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                        }
                        else
                        {
                            if (ef.Beg.Date > newDays[i].TVDate)
                            {
                                begTwips = Convert.ToInt32((24 * 60 * 60 + (ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                            }
                            else
                            {
                                begTwips = Convert.ToInt32(((ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60 - 24 * 60 * 60) / twipsCoef);
                            }
                        }
                        begTwips -= 3100;
                        timingTwips = Convert.ToInt32(ef.Timing / twipsCoef);
                        Efir tempEfir = MyStartupClass.getRTA(ef.Timing, ef.ITC);

                        //Timing
                        /*
                        string strTiming = TimeSpan.FromSeconds(ef.Timing).Hours + ":" + TimeSpan.FromSeconds(ef.Timing).Minutes + ":" + TimeSpan.FromSeconds(ef.Timing).Seconds;
                        if (strTiming.Right(2) == "00") strTiming.Substring(0, strTiming.Length - 3);
                        if (strTiming.Left(2) == "0:") strTiming.Substring(2);
                         */
                        string strTiming = getTimingString(ef.Timing);
                        //Text
                        curText += ef.Beg.ToString("HH:mm") + " - " + (ef.Beg + TimeSpan.FromSeconds(ef.Timing)).ToString("HH:mm") + " (" + strTiming + ")";
                        if (ef.Timing > 10 * 60)
                        {
                            curText += "\\line";
                        }
                        curText += ef.Title.ToUpper();

                        //Fill
                        if (ef.ProducerCode == "04" | ef.ProducerCode == "24" | (ef.ANR.ToLower().Contains("толстой") & ef.ANR.ToLower().Contains("воскресенье")))
                        {
                            fill = true;
                        }
                        else
                        {
                            fill = false;
                        }

                        //Infostring
                        curText += "   [" + tempEfir.getInfoString() + "] (" + ef.ProducerCode + ef.SellerCode + ")";

                        rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos, YPos: begTwips, XSize: xSize, YSize: timingTwips, Fill: fill));
                        //AgeCat
                        if (ef.Age >= 0)
                        {
                            curText = ef.Age.ToString() + "+";
                            rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos + xSize - 240, YPos: begTwips, XSize: 240, YSize: 150, Fill: false, Bold: true, Italic: false));
                        }
                    }
                    if (i == 4)
                    {
                        //Rightmost timescale
                        xPos = 190 + 5 * xSize + 5 * 200;
                        orbShift = 0;
                        rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: 0, left: false, orbShift: orbShift, xSize: xSize));
                    }
                }

            }
            return rtfList;
        }

        private static List<string> printBroadcast11(string dayVariantList = "")
        {
            double twipsCoef = 4.233;
            int xPos = 0;
            int xSize = 1250;
            int yPos = 650;
            int begTwips = 0;
            int timingTwips = 0;
            int fillR = 255;
            int fillG = 255;
            int fillB = 255;
            bool fill = false;
            string curText = "";
            string chName = "";
            int fontSize = 6;

            List<string> rtfList = new List<string>();
            List<Day> newDays = new List<Day>();
            newDays = getDaysFromHtml(dayVariantList);

            string dow = newDays[0].TVDate.ToString("dddd", russian).ToUpper();
            if (dow == "СРЕДА") dow = "СРЕДУ";
            if (dow == "ПЯТНИЦА") dow = "ПЯТНИЦУ";
            if (dow == "СУББОТА") dow = "СУББОТУ";

            //Header
            curText = "ПРОГРАММА ПЕРЕДАЧ ПЕРВОГО КАНАЛА НА " + dow + ", " + newDays[0].TVDate.ToString("dd/MM/yyyy");
            rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line: false, XPos: 190, YPos: 80, XSize: 16000, YSize: 230));

            for (int i = 0; i < 11; i++)
            {
                xPos = 190 + i * xSize + i * 200;
                int orbShift = 0;
                switch (i)
                {
                    case 0:
                        chName = "Калининград";
                        orbShift = -1;
                        break;
                    case 1:
                        chName = "Первый канал";
                        orbShift = 0;
                        break;
                    default:
                        orbShift = i-1;
                        chName = "ПК+" + (i - 1).ToString();

                        break;
                }

                ////Channel name
                //if (i == 0)
                //{
                //    curText = "Первый канал";
                //}
                //else
                //{
                //    curText = "Орбита " + (5 - i).ToString();
                //}
                //if (curText == "Орбита 4") curText += " (HD и SD)";
                rtfList.Add(rtfProg(Text: chName, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 400, XSize: xSize, YSize: 250));

                ////Orbit timeshift
                //int orbShift = 0;
                //orbShift = i * 2;

                //Left timescale
                rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: 0, left: true, orbShift: orbShift, xSize: xSize));
                if (newDays[i].Efirs != null)
                {
                    foreach (EfirType ef in newDays[i].Efirs)
                    {
                        curText = "";
                        if (ef.Beg.Date == newDays[i].TVDate)
                        {
                            begTwips = Convert.ToInt32(((ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                        }
                        else
                        {
                            if (ef.Beg.Date > newDays[i].TVDate)
                            {
                                begTwips = Convert.ToInt32((24 * 60 * 60 + (ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                            }
                            else
                            {
                                begTwips = Convert.ToInt32(((ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60 - 24 * 60 * 60) / twipsCoef);
                            }
                        }
                        begTwips -= 3100;
                        timingTwips = Convert.ToInt32(ef.Timing / twipsCoef);
                        Efir tempEfir = MyStartupClass.getRTA(ef.Timing, ef.ITC);

                        if (ef.Timing > 25 * 60)
                        {
                            fontSize = 6;
                        }
                        else
                        {
                            fontSize = 4;
                        }
                        //Timing
                        /*
                        string strTiming = TimeSpan.FromSeconds(ef.Timing).Hours + ":" + TimeSpan.FromSeconds(ef.Timing).Minutes + ":" + TimeSpan.FromSeconds(ef.Timing).Seconds;
                        if (strTiming.Right(2) == "00") strTiming.Substring(0, strTiming.Length - 3);
                        if (strTiming.Left(2) == "0:") strTiming.Substring(2);
                         */
                        string strTiming = getTimingString(ef.Timing);
                        //Text
                        curText += ef.Beg.ToString("HH:mm") + " - " + (ef.Beg + TimeSpan.FromSeconds(ef.Timing)).ToString("HH:mm") + " (" + strTiming + ")";
                        if (ef.Timing > 10 * 60)
                        {
                            curText += "\\line";
                        }
                        curText += ef.Title.ToUpper();

                        //Fill
                        if (ef.ProducerCode == "04" | ef.ProducerCode == "24" | (ef.ANR.ToLower().Contains("толстой") & ef.ANR.ToLower().Contains("воскресенье")))
                        {
                            fill = true;
                        }
                        else
                        {
                            fill = false;
                        }

                        if (i == 1 | i == 3 | i == 5 | i == 7 | i == 10)
                        {
                            if (fill)
                            {
                                fillR = 255;
                                fillG = 255;
                                fillB = 255;
                                
                            }
                            else
                            {
                                fillR = 225;
                                fillG = 225;
                                fillB = 225;
                            }
                        }
                        else
                        {
                            fillR = 255;
                            fillG = 255;
                            fillB = 255;
                        }

                        //Infostring
                        curText += "   [" + tempEfir.getInfoString() + "] (" + ef.ProducerCode + ef.SellerCode + ")";

                        rtfList.Add(rtfProg(FontSize: fontSize, Text: curText, XPos: xPos, YPos: begTwips, XSize: xSize, YSize: timingTwips, Fill: fill, fillR: fillR, fillG: fillG, fillB: fillB));
                        //AgeCat
                        if (ef.Age >= 0)
                        {
                            curText = ef.Age.ToString() + "+";
                            rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos + xSize - 240, YPos: begTwips, XSize: 240, YSize: 150, Fill: false, Bold: true, Italic: false, fillR: fillR, fillG: fillG, fillB: fillB));
                        }
                    }
                    rtfList.Add(rtfProg(Text: chName, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: Convert.ToInt32((30 * 60 * 60) / twipsCoef) - 3100, XSize: xSize, YSize: 250));
                    if (i == 10)
                    {
                        //Rightmost timescale
                        xPos = 190 + 11 * xSize + 11 * 200;
                        orbShift = 0;
                        rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: 0, left: false, orbShift: orbShift, xSize: xSize));
                    }
                }
                
            }
            return rtfList;
        }

        private static List<string> printRaskladka(string dayVariantList = "")
        {
            string curText = "";
            List<string> rtfList = new List<string>();
            Week curWeek = new Week();
            TVWeekType curTvWeek = new TVWeekType();
            int[] array_channel_codes = new int[1];
            array_channel_codes[0] = 10;

            //Print params
            int xSize = 15400;
            int xPos = 190;
            int dayNum = -1;
            double twipsCoef = 4.233;
            int yPosShift = 650;
            int begTwips = 0;
            int timingTwips = 0;
            bool fill = false;
            bool bold = false;
            bool italic = false;

            //Получаем список дней
            List<Day> newDayList = getDaysFromHtml(dayVariantList);

            //Header
            curText = "ПРОГРАММА ПЕРЕДАЧ ПЕРВОГО КАНАЛА С " + newDayList[0].TVDate.ToString("dd/MM/yyyy") + " ПО " + newDayList[newDayList.Count() - 1].TVDate.ToString("dd/MM/yyyy");
            rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line: false, XPos: 190, YPos: 40, XSize: 16000, YSize: 230));

            //Left timescale
            rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: yPosShift, left: true, orbShift: 0, xSize: xSize));

            xSize = 15400 / newDayList.Count();
            foreach (Day d in newDayList)
            {
                dayNum++;
                xPos = 190 + dayNum * xSize;
                //Dates
                curText = d.DoWRus;
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 400, XSize: xSize, YSize: 250));
                curText = d.TVDate.ToString("dd.MM.yyyy");
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 650, XSize: xSize, YSize: 250));
                //FullCap
                curText = d.FullCap;
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 900, XSize: xSize, YSize: 850));
                if (d.Efirs != null)
                {
                    foreach (EfirType ef in d.Efirs)
                    {
                        curText = "";
                        if (ef.Beg.Date == d.TVDate)
                        {
                            begTwips = Convert.ToInt32((ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                        }
                        else
                        {
                            if (ef.Beg.Date > d.TVDate)
                            {
                                begTwips = Convert.ToInt32((24 * 60 * 60 + ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                            }
                            else
                            {
                                begTwips = Convert.ToInt32((ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60 - 24 * 60 * 60) / twipsCoef);
                            }
                        }
                        begTwips -= 3100 - yPosShift;
                        timingTwips = Convert.ToInt32(ef.Timing / twipsCoef);
                        Efir tempEfir = MyStartupClass.getRTA(ef.Timing, ef.ITC);

                        //Timing
                        /*
                        string fullHours = "";
                        string fullMinutes = "";
                        string fullSeconds = "";
                        if (TimeSpan.FromSeconds(ef.Timing).Minutes < 10)
                        {
                            fullMinutes = "0" + TimeSpan.FromSeconds(ef.Timing).Minutes.ToString();                            
                        }
                        else
                        {
                            fullMinutes = TimeSpan.FromSeconds(ef.Timing).Minutes.ToString();
                        }
                        if (TimeSpan.FromSeconds(ef.Timing).Hours < 10)
                        {
                            fullHours = "0" + TimeSpan.FromSeconds(ef.Timing).Hours.ToString();                            
                        }
                        else
                        {
                            fullHours = TimeSpan.FromSeconds(ef.Timing).Hours.ToString();
                        }
                        if (TimeSpan.FromSeconds(ef.Timing).Seconds < 10)
                        {
                            fullSeconds = "0" + TimeSpan.FromSeconds(ef.Timing).Seconds.ToString();
                        }
                        else
                        {
                            fullSeconds = TimeSpan.FromSeconds(ef.Timing).Seconds.ToString();
                        }
                        
                        //string strTiming = TimeSpan.FromSeconds(ef.Timing).Hours + ":" + TimeSpan.FromSeconds(ef.Timing).Minutes + ":" + TimeSpan.FromSeconds(ef.Timing).Seconds;
                        
                        if (strTiming.Right(2) == "00") strTiming = strTiming.Substring(0, strTiming.Length - 3);
                        if (strTiming.Left(3) == "00:") strTiming = strTiming.Substring(3);
                         */
                        string strTiming = getTimingString(ef.Timing);
                        //Text
                        curText += ef.Beg.ToString("HH:mm") + " - " + (ef.Beg + TimeSpan.FromSeconds(ef.Timing)).ToString("HH:mm") + " (" + strTiming + ")";
                        curText += "\\line";
                        curText += ef.ANR.ToUpper();


                        //Fill
                        if (ef.ProducerCode == "04" | ef.ProducerCode == "24" | (ef.ANR.ToLower().Contains("толстой") & ef.ANR.ToLower().Contains("воскресенье")))
                        {
                            fill = true;
                        }
                        else
                        {
                            fill = false;
                        }
                        if (ef.ANR.ToLower().Contains("\\b"))
                        {
                            ef.ANR = ef.ANR.Replace("\\b0", "");
                            ef.ANR = ef.ANR.Replace("\\b1", "");
                            ef.ANR = ef.ANR.Replace("\\b", "");
                            bold = true;
                        }
                        else
                        {
                            bold = false;
                        }
                        if (ef.ANR.ToLower().Contains("\\i"))
                        {
                            ef.ANR = ef.ANR.Replace("\\i", "");
                            italic = true;
                        }
                        else
                        {
                            italic = false;
                        }


                        //Infostring
                        curText += "   [" + tempEfir.getInfoString() + "] (" + ef.ProducerCode + ef.SellerCode + ")";

                        rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos, YPos: begTwips, XSize: xSize, YSize: timingTwips, Fill: fill, Bold: bold, Italic: italic));

                        //AgeCat
                        if (ef.Age >= 0)
                        {
                            curText = ef.Age.ToString() + "+";
                            rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos + xSize - 240, YPos: begTwips, XSize: 240, YSize: 150, Fill: false, Bold: true, Italic: false));
                        }


                    }
                    if (dayNum == newDayList.Count() - 1)
                    {
                        //Rightmost timescale
                        xPos = 190 + (dayNum + 1) * xSize + 200;
                        rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: yPosShift, left: false, orbShift: 0, xSize: xSize));
                    }
                }

            }


            return rtfList;

        }

        private static List<string> printRatings(string dayVariantList = "")
        {
            string curText = "";
            List<string> rtfList = new List<string>();
            Week curWeek = new Week();
            TVWeekType curTvWeek = new TVWeekType();
            int[] array_channel_codes = new int[1];
            array_channel_codes[0] = 10;

            //Print params
            int xSize = 15400;
            int xPos = 190;
            int dayNum = -1;
            double twipsCoef = 4.233;
            int yPosShift = 0;//650;
            int begTwips = 0;
            int timingTwips = 0;
            bool fill = false;
            bool bold = false;
            bool italic = false;

            //Получаем список дней
            List<Day> newDayList = getRatingsDaysFromHtml(dayVariantList);

            //Header
            curText = "ДОЛИ ТЕЛЕПЕРЕДАЧ НА " + newDayList[0].TVDate.ToString("dd/MM/yyyy") + " (СТИ+ / Mediascope-Mos (18+) / Mediascope-Rus (18+)";
            rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line: false, XPos: 190, YPos: 20, XSize: 16000, YSize: 230));

            //Left timescale
            rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: yPosShift, left: true, orbShift: 0, xSize: xSize));

            xSize = 15400 / newDayList.Count();
            foreach (Day d in newDayList)
            {
                dayNum++;
                xPos = 190 + dayNum * xSize;
                //Channel names                
                switch (d.KanalKod)
                {
                    case 10:
                        curText = "Первый канал";
                        break;
                    case 21:
                        curText = "Россия-1";
                        break;
                    case 40:
                        curText = "НТВ";
                        break;
                    case 52:
                        curText = "СТС";
                        break;
                    case 51:
                        curText = "ТНТ";
                        break;
                    case 53:
                        curText = "ТВЦ";
                        break;                    
                }
                rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 400, XSize: xSize, YSize: 250));
                //curText = d.TVDate.ToString("dd.MM.yyyy");
                //rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 650, XSize: xSize, YSize: 250));
                //FullCap
                //curText = d.FullCap;
                //rtfList.Add(rtfProg(Text: curText, FontSize: 8, Bold: true, Line: true, XPos: xPos, YPos: 900, XSize: xSize, YSize: 850));
                if (d.RatEfirs != null)
                {
                    foreach (Efir ef in d.RatEfirs)
                    {
                        curText = "";
                        if (ef.Beg.Date == d.TVDate)
                        {
                            begTwips = Convert.ToInt32((ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                        }
                        else
                        {
                            if (ef.Beg.Date > d.TVDate)
                            {
                                begTwips = Convert.ToInt32((24 * 60 * 60 + ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                            }
                            else
                            {
                                begTwips = Convert.ToInt32((ef.Beg.Hour * 60 * 60 + ef.Beg.Minute * 60 - 24 * 60 * 60) / twipsCoef);
                            }
                        }
                        begTwips -= 3100 - yPosShift;
                        timingTwips = Convert.ToInt32(ef.Timing / twipsCoef);
                        Efir tempEfir = MyStartupClass.getRTA(ef.Timing, ef.ITC);

                        string strTiming = getTimingString(ef.Timing);
                        //Text
                        curText += ef.Beg.ToString("HH:mm") + " - " + ef.EndTime.ToString("HH:mm");
                        curText += "\\line";
                        if (ef.ANR.Length>0)
                        {
                            if (ef.ANR.Substring(0,1)!="\"")
                            {
                                curText += "\""+ef.ANR.ToUpper()+"\"";
                            }
                            else
                            {
                                curText += ef.ANR.ToUpper();
                            }
                        }
                        


                        int red = 255;
                        int green = 255;
                        int blue = 255;
                        //Fill
                        if (ef.Foot.Length>0 & ef.Foot.Contains(','))
                        {
                            fill = true;
                            string[] rgb = ef.Foot.Split(',');
                            red = Convert.ToInt16(rgb[0]);
                            green = Convert.ToInt16(rgb[1]);
                            blue = Convert.ToInt16(rgb[2]);
                        }
                        else
                        {
                            fill = false;
                        }
                        

                        rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos, YPos: begTwips, XSize: xSize, YSize: timingTwips, Fill: fill, Bold: bold, Italic: italic, fillR: red,fillG: green,fillB: blue));


                        //Ratings
                        if (ef.Timing > 5 * 60)
                        {
                            if (ef.DR > 0)
                            {
                                curText = ef.DR.ToString("{0.0}");
                            }
                            else
                            {
                                curText = "-";
                            }
                            rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos + xSize - 250, YPos: begTwips + timingTwips - 160, XSize: 250, YSize: 160, Fill: false, Bold: true, Italic: false));
                            if (ef.DM > 0)
                            {
                                curText = ef.DM.ToString("{0.0}");
                            }
                            else
                            {
                                curText = "-";
                            }
                            rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos + xSize - 250 * 2, YPos: begTwips + timingTwips - 160, XSize: 250, YSize: 160, Fill: false, Bold: true, Italic: false));
                            if (ef.DSti > 0)
                            {
                                curText = ef.DSti.ToString("{0.0}");
                            }
                            else
                            {
                                curText = "-";
                            }
                            rtfList.Add(rtfProg(FontSize: 6, Text: curText, XPos: xPos + xSize - 250 * 3, YPos: begTwips + timingTwips - 160, XSize: 250, YSize: 160, Fill: false, Bold: true, Italic: false));
                        }
                    }
                    if (dayNum == newDayList.Count() - 1)
                    {
                        //Rightmost timescale
                        xPos = 190 + (dayNum + 1) * xSize + 200;
                        rtfList.Add(rtfTimeScale(xPos: xPos, yPosShift: yPosShift, left: false, orbShift: 0, xSize: xSize));
                    }
                }

            }


            return rtfList;

        }

        private static List<Day> getDaysFromHtml(string daysVariantsString = "")
        {
            List<Day> dayList = new List<Day>();
            string curStr = daysVariantsString;
            List<Tuple<DateTime, int, int>> dateVarList = new List<Tuple<DateTime, int, int>>();
            char[] sep;
            //Костыль для ртф на планшете
            if (daysVariantsString.Contains(";"))
            {
                sep = ";".ToArray();
            }
            else
            {
                sep = "%3B".ToArray(); 
            }
            
            string[] splitArray = daysVariantsString.Split(sep);

            foreach (string s in splitArray)
            {
                string[] splitArray2 = s.Split('_');
                dateVarList.Add(new Tuple<DateTime, int, int>(DateTime.Parse(splitArray2[0]), Convert.ToInt16(splitArray2[1]), Convert.ToInt16(splitArray2[2])));
            }

            if (dateVarList.Count() > 0)
            {
                foreach (Tuple<DateTime, int, int> tDV in dateVarList)
                {
                    Day curDay = new Day();
                    curDay.TVDate = tDV.Item1;
                    curDay.VariantKod = tDV.Item2;
                    curDay.KanalKod = tDV.Item3;
                    curDay.Efirs = wc.GetEfirs(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                    TVDayVariantParam curDayParam = wc.GetVarTVDayParam(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                    curDay.FullCap = curDayParam.Cap;
                    if (curDay.FullCap.Length > 0)
                    {
                        curDay.FullCap += "\n";
                    }
                    curDay.FullCap += curDayParam.MemoryDates;
                    curDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
                    curDay.DoWRus = char.ToUpper(curDay.DoWRus[0]) + curDay.DoWRus.Substring(1);
                    dayList.Add(curDay);
                }
            }

            return dayList;
        }

        private static List<Day> getRatingsDaysFromHtml(string daysVariantsString = "")
        {
            List<Day> dayList = new List<Day>();            

            string curStr = daysVariantsString;
            List<Tuple<DateTime, int, decimal,decimal,int>> dateVarList = new List<Tuple<DateTime, int, decimal, decimal, int>>();
            char[] sep;
            //Костыль для ртф на планшете
            if (daysVariantsString.Contains(";"))
            {
                sep = ";".ToArray();
            }
            else
            {
                sep = "%3B".ToArray();
            }

            string[] splitArray = daysVariantsString.Split(sep);

            foreach (string s in splitArray)
            {
                string[] splitArray2 = s.Split('_');
                dateVarList.Add(new Tuple<DateTime, int, decimal, decimal, int>(DateTime.Parse(splitArray2[0]), Convert.ToInt16(splitArray2[1]), Convert.ToDecimal(splitArray2[2]), Convert.ToDecimal(splitArray2[3]), Convert.ToInt16(splitArray2[4])));
            }



            if (dateVarList.Count() > 0)
            {
                int i = 0;
                foreach (Tuple<DateTime, int, decimal, decimal,int> tDV in dateVarList)
                {
                    Day newDay = new Day();
                    newDay.TVDate = tDV.Item1;
                    switch (i)
                    {
                        case 0:
                            newDay.KanalKod = 10;
                            break;
                        case 1:
                            newDay.KanalKod = 21;
                            break;
                        case 2:
                            newDay.KanalKod = 40;
                            break;
                        case 3:
                            newDay.KanalKod = 52;
                            break;
                        case 4:
                            newDay.KanalKod = 51;
                            break;
                        case 5:
                            newDay.KanalKod = 53;
                            break;
                    }
                    CultureInfo russian = new CultureInfo("ru-RU");
                    newDay.DoWRus = newDay.TVDate.ToString("dddd", russian);
                    newDay.DoWRus = char.ToUpper(newDay.DoWRus[0]) + newDay.DoWRus.Substring(1);
                    RatEfirType[] ratEfirs = wc.GetRatEfirs(newDay.KanalKod, newDay.TVDate);
                    newDay.RatEfirs = getRatEfirTypeArrayRatings(ratEfirs,tDV);
                    //Control share -> Chk
                    if (tDV.Item2 == 1) { newDay.Chk = true; } else { newDay.Chk = false; }
                    //Base share -> Cap
                    newDay.Cap = tDV.Item3.ToString();
                    //Step share -> FullCap
                    newDay.FullCap = tDV.Item4.ToString();
                    //Service -> Foot
                    newDay.Foot = tDV.Item5.ToString();
                    dayList.Add(newDay);
                    i++;
                }
            }
            return dayList;
        }

        private static List<Day> getSvodkaDaysFromHtml(string daysVariantsString = "")
        {
            List<Day> dayList = new List<Day>();
            string curStr = daysVariantsString;
            //int startInt = 0;

            List<Tuple<DateTime, int, int>> dateVarList = new List<Tuple<DateTime, int, int>>();

            string[] splitArray = daysVariantsString.Split(';');
            foreach (string s in splitArray)
            {
                string[] splitArray2 = s.Split('_');
                dateVarList.Add(new Tuple<DateTime, int, int>(DateTime.Parse(splitArray2[0]), Convert.ToInt16(splitArray2[1]), Convert.ToInt16(splitArray2[2])));
            }

            if (dateVarList.Count() > 0)
            {
                foreach (Tuple<DateTime, int, int> tDV in dateVarList)
                {
                    Day curDay = new Day();
                    curDay.TVDate = tDV.Item1;
                    curDay.VariantKod = tDV.Item2;
                    curDay.KanalKod = tDV.Item3;
                    curDay.Efirs = chopper1.Controllers.DayController.getEfirTypeArraySvodka(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                    TVDayVariantParam curDayParam = wc.GetVarTVDayParam(curDay.TVDate, curDay.KanalKod, curDay.VariantKod);
                    curDay.FullCap = curDayParam.Cap;
                    if (curDay.FullCap.Length > 0)
                    {
                        curDay.FullCap += "\n";
                    }
                    curDay.FullCap += curDayParam.MemoryDates;
                    curDay.DoWRus = curDay.TVDate.ToString("dddd", russian);
                    curDay.DoWRus = char.ToUpper(curDay.DoWRus[0]) + curDay.DoWRus.Substring(1);
                    dayList.Add(curDay);
                }
            }

            return dayList;
        }


        private static string rtfTimeScale(int xPos = 0, int yPosShift = 0, bool left = true, int orbShift = 0, int xSize = 0)
        {
            string curLine = "";
            string labelText = "";
            int startHour = 5 - orbShift;
            int endHour = 30 - orbShift;
            int curXPos = 0;
            int yPos = 0;
            int len = 0;
            int width = 5;
            bool gor = true;
            bool punktir = false;
            bool bold = false;
            int zOrder = 1000;



            //Vertical line
            if (left)
            {
                yPos = getTwipsPositionByTime(4 * 60 * 60 + 08 * 60);
                curLine += rtfLine(false, xPos, yPos + yPosShift, 22000, punktir, bold, width, zOrder);
                //curLine += rtfLine(false, xPos + xSize, yPos + yPosShift, 22000, punktir, bold, width, zOrder);
            }
            else
            {
                yPos = getTwipsPositionByTime(4 * 60 * 60 + 08 * 60) + yPosShift;
                curLine += rtfLine(false, xPos - 200, yPos + yPosShift, 22000, punktir, bold, width, zOrder);
            }
            xPos -= 200;




            for (int hHour = startHour; hHour < endHour; hHour++)
            {
                for (int dec = 0; dec < 6; dec++)
                {
                    yPos = getTwipsPositionByTime((hHour + orbShift) * 60 * 60 + dec * 10 * 60) + yPosShift;
                    if (dec == 0 | dec == 3)
                    {
                        len = 200;
                        curXPos = xPos;
                        //Подпись часа
                        if (dec == 0)
                        {
                            if (hHour >= 0)
                            {
                                if (hHour < 24)
                                {
                                    labelText = hHour.ToString();
                                }
                                else
                                {
                                    labelText = (hHour - 24).ToString();
                                }
                            }
                            else
                            {
                                labelText = (24 + hHour).ToString();
                            }
                            if (labelText.Length == 1) labelText = "0" + labelText;

                            curLine += rtfProg(FontSize: 6, Text: labelText, XPos: xPos, YPos: yPos, XSize: 200, YSize: 200, Line: false, Bold: true);

                        }
                    }
                    else
                    {
                        len = 100;
                        if (left)
                        {
                            curXPos = xPos + 100;
                        }
                        else
                        {
                            curXPos = xPos;
                        }
                    }
                    curLine += rtfLine(gor, curXPos, yPos, len, punktir, bold, width, zOrder);
                }
            }


            return curLine;
        }



        private static int getTwipsPositionByTime(int begTime)
        {
            int yPos = 0;
            double twipsCoef = 4.233;
            yPos = Convert.ToInt32(begTime / twipsCoef) - 3100;

            return yPos;
        }

        

        private static string rtfLine(bool gor, int xPos, int yPos, int len, bool punktir, bool Bold = false, int width = 5, int zOrder = 1000)
        {
            string curLine = "";
            curLine += "{\\*\\do\\dobxmargin\\dobymargin\\dodhgt";
            curLine += zOrder.ToString();
            curLine += "\\dolock\\dpline\\dpptx0\\dppty0\\dpptx";
            if (gor)
            {
                curLine += (len - 1).ToString();
            }
            else
            {
                curLine += "0";
            }
            curLine += "\\dppty";
            if (gor)
            {
                curLine += "0";

            }
            else
            {
                curLine += (len - 1).ToString();
            }
            curLine += "\\dpx" + xPos.ToString();
            curLine += "\\dpy" + yPos.ToString();
            curLine += "\\dpxsize";
            if (gor)
            {
                curLine += len.ToString();
            }
            else
            {
                curLine += "1";
            }
            curLine += "\\dpysize";
            if (gor)
            {
                curLine += "1";
            }
            else
            {
                curLine += len.ToString();
            }
            curLine += "\\dpline";
            if (punktir)
            {
                curLine += "dot";
            }
            else
            {
                curLine += "solid";
            }
            curLine += "\\dplinecor0\\dplinecog0\\dplinecob0\\dplinew";
            curLine += width;
            curLine += "}";


            curLine += "\n";

            return curLine;
        }


        private static string rtfHead()
        {
            string curLine = "";
            //Head
            curLine += "{\\rtf1\\ansi \\deff4\\deflang1033{\\fonttbl{\\f1\\fcharset204\\fswiss Arial;}{\\f4\\froman\\fcharset204";
            curLine += "\\fprq2 Times New Roman Cyr;}{\\f5\\fswiss\\fcharset204\\fprq2 Arial Cyr;}}";
            curLine += "{\\stylesheet{\\f4\\lang1049 \\snext0 Normal;}{\\*";
            curLine += "\\cs10 \\additive Default Paragraph Font;}}";
            curLine += "\n";
            curLine += "{\\colortbl;\\red0\\green0\\blue0;}";
            curLine += "\\paperw16840\\paperh23814";
            curLine += "\\margl400";
            curLine += "\\margr400";
            curLine += "\\margt400";
            curLine += "\\margb400";
            curLine += "\\deftab708";
            curLine += "\\widowctrl\\ftnbj\\aenddoc\\hyphhotz425\\hyphcaps0\\formshade \\fet0";
            curLine += "\\sectd \\psz9\\linex0\\headery709\\footery709\\colsx709\\endnhere";
            curLine += "\\pard\\plain \\s17 \\fs24\\f4\\lang1024";
            curLine += "\n";

            return curLine;
        }

        private static string rtfTableHeader()
        {
            string curLine = "";

            

            return curLine;
        }


        private static string rtfFoot()
        {
            string curLine = "";
            //Foot
            curLine += "\\par }";
            curLine += "\n";

            return curLine;
        }


        public static string getTimingString (int timingSec)
        {           
            string fullHours = "";
            string fullMinutes = "";
            string fullSeconds = "";
            if (TimeSpan.FromSeconds(timingSec).Minutes < 10)
            {
                fullMinutes = "0" + TimeSpan.FromSeconds(timingSec).Minutes.ToString();
            }
            else
            {
                fullMinutes = TimeSpan.FromSeconds(timingSec).Minutes.ToString();
            }
            if (TimeSpan.FromSeconds(timingSec).Hours < 10)
            {
                fullHours = "0" + TimeSpan.FromSeconds(timingSec).Hours.ToString();
            }
            else
            {
                fullHours = TimeSpan.FromSeconds(timingSec).Hours.ToString();
            }
            if (TimeSpan.FromSeconds(timingSec).Seconds < 10)
            {
                fullSeconds = "0" + TimeSpan.FromSeconds(timingSec).Seconds.ToString();
            }
            else
            {
                fullSeconds = TimeSpan.FromSeconds(timingSec).Seconds.ToString();
            }

            //string strTiming = TimeSpan.FromSeconds(ef.Timing).Hours + ":" + TimeSpan.FromSeconds(ef.Timing).Minutes + ":" + TimeSpan.FromSeconds(ef.Timing).Seconds;
            string strTiming = fullHours + ":" + fullMinutes + ":" + fullSeconds;
            if (strTiming.Right(2) == "00") strTiming = strTiming.Substring(0, strTiming.Length - 3);
            if (strTiming.Left(3) == "00:") strTiming = strTiming.Substring(3);


            return strTiming;
        }

        public static void getRtf()
        {
            string curLine;
            List<string> rtfList = new List<string>();

            //Head
            curLine = "{\\rtf1\\ansi \\deff4\\deflang1033{\\fonttbl{\\f4\\froman\\fcharset204";
            rtfList.Add(curLine);
            curLine = "\\fprq2 Times New Roman Cyr;}{\\f5\\fswiss\\fcharset204\\fprq2 Arial Cyr;}}";
            rtfList.Add(curLine);
            curLine = "{\\stylesheet{\\f4\\lang1049 \\snext0 Normal;}{\\*";
            rtfList.Add(curLine);
            curLine = "\\cs10 \\additive Default Paragraph Font;}}";
            rtfList.Add(curLine);
            curLine = "";
            rtfList.Add(curLine);
            curLine = "{\\colortbl;\\red0\\green0\\blue0;}";
            rtfList.Add(curLine);
            curLine = "\\paperw16840\\paperh23814";
            rtfList.Add(curLine);
            curLine = "\\margl400";
            rtfList.Add(curLine);
            curLine = "\\margr400";
            rtfList.Add(curLine);
            curLine = "\\margt400";
            rtfList.Add(curLine);
            curLine = "\\margb400";
            rtfList.Add(curLine);
            curLine = "\\deftab708";
            rtfList.Add(curLine);
            curLine = "\\widowctrl\\ftnbj\\aenddoc\\hyphhotz425\\hyphcaps0\\formshade \\fet0";
            rtfList.Add(curLine);
            curLine = "\\sectd \\psz9\\linex0\\headery709\\footery709\\colsx709\\endnhere";
            rtfList.Add(curLine);
            curLine = "\\pard\\plain \\s17 \\fs24\\f4\\lang1024";
            rtfList.Add(curLine);
            curLine = "";
            rtfList.Add(curLine);
            //Progs

            rtfList.Add(rtfProg());


            //Foot
            curLine = "\\par }";
            rtfList.Add(curLine);
            curLine = "";
            rtfList.Add(curLine);



            //string[] lines = { "First line", "Second line", "Third line" };
            string[] rtfArray = rtfList.ToArray();

            System.IO.File.WriteAllLines(@"C:\WriteLines.rtf", rtfArray, Encoding.GetEncoding(1251));


        }

        private static string rtfStolbyRow(int FontSize = 10, string Text = "Проверка", int rowHeight = 20, int cellWidth1 = 50, int cellWidth2 = 500, int cellWidth3 = 50, bool rightBorder = false, bool bold = false, bool fill = false, bool italic = false )
        {
            string rowLine = "";

            return rowLine;
        }
        private static Efir[] getRatEfirTypeArrayRatings(RatEfirType[] ratEfirs, Tuple<DateTime, int, decimal, decimal, int> tDV = null)
        {
            List<Efir> efirs = new List<Efir>();

            foreach (RatEfirType r in ratEfirs)
            {
                Efir newEfir = new Efir();
                newEfir.Beg = r.Beg;
                newEfir.EndTime = r.End;
                TimeSpan ts = r.End - r.Beg;
                newEfir.Timing = Convert.ToInt32(ts.TotalSeconds);
                //newEfir.Timing = r.Timing;
                newEfir.Title = r.Title;
                newEfir.ANR = r.Title;
                newEfir.ProducerCode = "";
                newEfir.SellerCode = "";
                newEfir.Age = 0;
                newEfir.AgeCat = "";
                newEfir.TVDayRef = "";
                newEfir.Cap = "";
                newEfir.Foot = "";
                //Рейтинги
                newEfir.DSti = r.DSTI;
                newEfir.DM = r.DM;
                newEfir.DR = r.DR;
                newEfir.RM = r.RM;
                newEfir.RR = r.RR;

                if (tDV != null)
                {
                    if (tDV.Item2 == 1)
                    {
                        switch (tDV.Item5)
                        {
                            case 1:
                                newEfir.Foot = getRGBstring(newEfir.DSti, tDV.Item3, tDV.Item4);
                                break;
                            case 3:
                                newEfir.Foot = getRGBstring(newEfir.DM, tDV.Item3, tDV.Item4);
                                break;
                            case 5:
                                newEfir.Foot = getRGBstring(newEfir.DR, tDV.Item3, tDV.Item4);
                                break;
                        }
                    }
                }

                efirs.Add(newEfir);
            }

            

            return efirs.ToArray();
        }

        private static string getRGBstring (decimal share, decimal baseShare, decimal step)
        {
            string rgb = "";
            var c = Startup.context.Settings.Where(x => x.UserName == "Global").FirstOrDefault();            
            if (share >= baseShare & share < baseShare+step*1) { rgb = c.Green1; }
            if (share >= baseShare+step*1 & share < baseShare + step * 2) { rgb = c.Green2; }
            if (share >= baseShare + step * 2) { rgb = c.Green3; }
            if (share < baseShare & share >= baseShare-step*1) { rgb = c.Red1; }
            if (share < baseShare-step*1 & share >= baseShare - step * 2) { rgb = c.Red2; }
            if (share>0 & share < baseShare - step * 2) { rgb = c.Red3; }
            return rgb;            
        }


        private static string rtfProg(int Inside = 10, int FontSize = 8, string Text = "Проверка", int XPos = 1000, int YPos = 1000, int XSize = 2000, int YSize = 2000, bool Line = true, bool Bold = false, bool Fill = false, string Format = "", bool Italic = false, int dodhgt = 1, int fillR=255, int fillG=255, int fillB=255)
        {
            string progLine = "";
            if (!(Text == "18+" | Text == "16+" | Text == "12+" | Text == "0+") & Text.Length>2)
            {
                if (YSize <= 16 * 60 / 4.233) FontSize = 5;
                if (YSize <= 11 * 60 / 4.233) FontSize = 4;
                if (YSize <= 5 * 60 / 4.233) FontSize = 3;                
            }

            string cur_key;
            if (Text.Contains("$Ш") || Text.Contains("$Х") || Text.Contains("$X") || Text.Contains("$C") || Text.Contains("$С") || Text.Contains("$Ц"))
            {
                while (Text.IndexOf("$") >= 0)
                {
                    //Размер шрифта
                    if (Text.Substring(Text.IndexOf("$") + 1, 1) == "Ш")
                    {
                        cur_key = Text.Substring(Text.IndexOf("$"), 3);
                        FontSize = Convert.ToInt32(cur_key.Right(1)) - 1;
                        //FontSize = Convert.ToInt32(cur_key.Right(1))/2;
                        Text = Text.Replace(cur_key, "");
                    }                   
                    //Загадочный ключ
                    if (Text.IndexOf("$") >= 0 & (Text.Substring(Text.IndexOf("$") + 1, 1) == "X" || Text.Substring(Text.IndexOf("$") + 1, 1) == "Х"))
                    {
                        cur_key = Text.Substring(Text.IndexOf("$"), 2);
                        //curEfir.Reserv = true;
                        Text = Text.Replace(cur_key, "");
                    }
                    //Заливка
                    if (Text.IndexOf("$") >= 0 & (Text.Substring(Text.IndexOf("$") + 1, 1) == "C" || Text.Substring(Text.IndexOf("$") + 1, 1) == "С"))
                    {

                        cur_key = Text.Substring(Text.IndexOf("$"), 4);
                        Fill = true;
                        Text = Text.Replace(cur_key, "");
                    }

                }
            }
            if (!Text.Contains("$Ш")& !Text.Contains("ПРОГРАММА ПЕРЕДАЧ"))
            {
                if (Text.Length>15 & YSize <=20*60/4.233 & FontSize >4)
                {
                    FontSize = 4;
                }
                if (Text.Length > 15 & YSize <= 40 * 60 / 4.233 & FontSize > 5)
                {
                    FontSize = 5;
                }
            }
                



            Text = Text.Replace("\n", "\\line");
            Text = Text.Replace("#", "\\line");

            progLine += "{\\*\\do\\dobxmargin\\dobymargin\\dodhgt";
            progLine += dodhgt.ToString();
            progLine += "\\dolock\\dptxbx\\dptxbxmar";
            //nInsideN - Internal margin of the text box.
            progLine += Inside.ToString();
            progLine += "{\\dptxbxtext \\pard\\plain \\qc \\f5\\cf1\\lang1049{\\fs";
            progLine += (FontSize * 2).ToString();
            if (Bold) progLine += "\\b";
            if (Italic) progLine += "\\i";
            progLine += " ";
            progLine += Text;
            progLine += "\\line";
            progLine += "\\par }}\\dpx";
            progLine += XPos.ToString();
            progLine += "\\dpy";
            progLine += YPos.ToString();
            progLine += "\\dpxsize";
            progLine += XSize.ToString();
            progLine += "\\dpysize";
            progLine += YSize.ToString();
            progLine += "\\dpline";
            if (Line)
            {
                progLine += "solid";
            }
            else
            {
                progLine += "hollow";
            }
            progLine += "\\dplinecor0\\dplinecog0\\dplinecob0\\dplinew";
            //dplinewN - Thickness of line (in twips)
            progLine += "2";
            progLine += "\\dpfillfgcr255\\dpfillfgcg255\\dpfillfgcb255";
            if (Fill)
            {
                if (fillR < 255 | fillG < 255 | fillB < 255)
                {
                    progLine += ("\\dpfillbgcr" + fillR.ToString() + "\\dpfillbgcg" + fillG.ToString() + "\\dpfillbgcb" + fillB.ToString() + "\\dpfillpat1").Replace(" ", "");
                }
                else
                {
                    progLine += "\\dpfillbggray50\\dpfillpat1";
                    
                }
            }
            else
            {
                if (fillR < 255 | fillG < 255 | fillB < 255)
                {
                    progLine += ("\\dpfillbgcr" + fillR.ToString() + "\\dpfillbgcg" + fillG.ToString() + "\\dpfillbgcb" + fillB.ToString() + "\\dpfillpat1").Replace(" ", "");
                }
                else
                {
                    progLine += "\\dpfillbgcr0\\dpfillbgcg0\\dpfillbgcb0\\dpfillpat0";
                }
            }
            progLine += "}";
            progLine += "\n";

            return progLine;
        }

    }
}
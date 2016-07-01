using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;
using chopper1.Models;
using System.Web.Mvc;
using System.Text;
using System.Globalization;

using Microsoft.Office.Interop.Word;
using System.Reflection;

namespace chopper1
{

    public class MyStartupClass
    {
        public static WebСервис1 wc = new WebСервис1();
        public static int selectedID = 2;
        public static TVWeekType[] tvWeeks;
        public static int lastNewsStart = 0;
        public static int totalBlockDur = 0;
        public static List<chopper1.Models.Day> days_to_check = new List<chopper1.Models.Day>();
        public static List<TVDayVariantT> variants_to_check = new List<TVDayVariantT>();
        public static List<TVDayVariantT> variants_to_update = new List<TVDayVariantT>();
        public static string curCatConnection = "PlanCatConnection";

        public static int[] fullChannelCodesArray = new int[] { 10, 11, 12, 13, 14 };



        /*
        private static void CreateDocument()
        {
            try
            {
                //Create an instance for word app
                Microsoft.Office.Interop.Word.Application winword = new Microsoft.Office.Interop.Word.Application();

                //Set animation status for word application
                winword.ShowAnimation = false;

                //Set status for word application is to be visible or not.
                winword.Visible = false;

                //Create a missing variable for missing value
                object missing = System.Reflection.Missing.Value;

                //Create a new document
                Microsoft.Office.Interop.Word.Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                //Add header into the document
                foreach (Microsoft.Office.Interop.Word.Section section in document.Sections)
                {
                    //Get the header range and add the header details.
                    Microsoft.Office.Interop.Word.Range headerRange = section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage);
                    headerRange.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = Microsoft.Office.Interop.Word.WdColorIndex.wdBlue;
                    headerRange.Font.Size = 10;
                    headerRange.Text = "Header text goes here";
                }

                //Add the footers into the document
                foreach (Microsoft.Office.Interop.Word.Section wordSection in document.Sections)
                {
                    //Get the footer range and add the footer details.
                    Microsoft.Office.Interop.Word.Range footerRange = wordSection.Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    footerRange.Font.ColorIndex = Microsoft.Office.Interop.Word.WdColorIndex.wdDarkRed;
                    footerRange.Font.Size = 10;
                    footerRange.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    footerRange.Text = "Footer text goes here";
                }

                //adding text to document
                document.Content.SetRange(0, 0);
                document.Content.Text = "This is test document " + Environment.NewLine;

                //Add paragraph with Heading 1 style
                Microsoft.Office.Interop.Word.Paragraph para1 = document.Content.Paragraphs.Add(ref missing);
                object styleHeading1 = "Heading 1";
                para1.Range.set_Style(ref styleHeading1);
                para1.Range.Text = "Para 1 text";
                para1.Range.InsertParagraphAfter();

                //Add paragraph with Heading 2 style
                Microsoft.Office.Interop.Word.Paragraph para2 = document.Content.Paragraphs.Add(ref missing);
                object styleHeading2 = "Heading 2";
                para2.Range.set_Style(ref styleHeading2);
                para2.Range.Text = "Para 2 text";
                para2.Range.InsertParagraphAfter();

                //Create a 5X5 table and insert some dummy record
                Table firstTable = document.Tables.Add(para1.Range, 5, 5, ref missing, ref missing);

                firstTable.Borders.Enable = 1;
                foreach (Row row in firstTable.Rows)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        //Header row
                        if (cell.RowIndex == 1)
                        {
                            cell.Range.Text = "Column " + cell.ColumnIndex.ToString();
                            cell.Range.Font.Bold = 1;
                            //other format properties goes here
                            cell.Range.Font.Name = "verdana";
                            cell.Range.Font.Size = 10;
                            //cell.Range.Font.ColorIndex = WdColorIndex.wdGray25;                            
                            cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                            //Center alignment for the Header cells
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                        }
                        //Data row
                        else
                        {
                            cell.Range.Text = (cell.RowIndex - 2 + cell.ColumnIndex).ToString();
                        }
                    }
                }

                //Save the document
                object filename = @"c:\temp1.docx";
                document.SaveAs2(ref filename);
                document.Close(ref missing, ref missing, ref missing);
                document = null;
                winword.Quit(ref missing, ref missing, ref missing);
                winword = null;
                //MsgBox.Show("Document created successfully !");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        */


        public static void printReport(string repType = "Broadcast", string pointer = "2016-07-06", bool pdf = false, bool word = true, bool print = false)
        {   
            double twipsCoef = 4.233;
            int xPos = 0;
            int xSize = 2900;
            int yPos = 650;
            int begTwips = 0;
            int timingTwips = 0;
            bool fill = false;
            string curText = "";

            CultureInfo russian = new CultureInfo("ru-RU"); 

            List<string> rtfList = new List<string>();
            string path = "C:\\";
            string fileName = repType + pointer + "_"+DateTime.Now.ToString("HHmmss")+".rtf";
            string fullPath = path+fileName;
            

            rtfList.Add(rtfHead());


            //Недельная раскладка
            if (repType=="Raskladka")
            {
            
            }

            //Соколовский вариант
            //Пока берем из базы по дню
            if (repType=="Broadcast")
            {
                

                DateTime curDate = DateTime.Parse(pointer);
                List<Day> newDays = new List<Day>();

                int[] array_channel_codes = new int[5];
                array_channel_codes[0] = 10;
                array_channel_codes[1] = 14;
                array_channel_codes[2] = 13;
                array_channel_codes[3] = 12;
                array_channel_codes[4] = 11;

                //Пока только 1 вариант!!!
                int variantNum = 1;

                foreach (int chCode in array_channel_codes)
                {
                    newDays.Add(MyStartupClass.getDayByDateAndVariantCode(curDate, variantNum, chCode));
                }

                string dow = newDays[0].TVDate.ToString("dddd", russian).ToUpper();
                if (dow == "СРЕДА") dow = "СРЕДУ";
                if (dow == "ПЯТНИЦА") dow = "ПЯТНИЦУ";
                if (dow == "СУББОТА") dow = "СУББОТУ";

                //Header
                curText = "ПРОГРАММА ПЕРЕДАЧ ПЕРВОГО КАНАЛА НА " + dow +", " + newDays[0].TVDate.ToString("dd/MM/yyyy");
                rtfList.Add(rtfProg(Text: curText, FontSize: 11, Bold: true, Line:false, XPos: 190, YPos: 80, XSize: 16000, YSize: 230));




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
                    rtfList.Add(rtfTimeScale(xPos, left: true, orbShift: orbShift, xSize: xSize));
                    if (newDays[i].Efirs != null)
                    {
                        foreach (EfirType ef in newDays[i].Efirs)
                        {
                            curText = "";
                            if (ef.Beg.Date == curDate.Date)
                            {
                                begTwips = Convert.ToInt32(((ef.Beg.Hour + orbShift) * 60 * 60 + ef.Beg.Minute * 60) / twipsCoef);
                            }
                            else
                            {
                                if (ef.Beg.Date > curDate.Date)
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
                            Efir tempEfir = getRTA(ef.Timing, ef.ITC);

                            //Timing
                            string strTiming = TimeSpan.FromSeconds(ef.Timing).Hours + ":" + TimeSpan.FromSeconds(ef.Timing).Minutes + ":" + TimeSpan.FromSeconds(ef.Timing).Seconds;
                            if (strTiming.Right(2) == "00") strTiming.Substring(0, strTiming.Length - 3);
                            if (strTiming.Left(2) == "0:") strTiming.Substring(2);
                            //Text
                            curText += ef.Beg.ToString("HH:mm") + " - " + (ef.Beg + TimeSpan.FromSeconds(ef.Timing)).ToString("HH:mm") + " (" + strTiming + ")";
                            curText += "\\line";
                            curText += ef.Title.ToUpper();

                            //Fill
                            if (ef.ProducerCode == "04" | ef.ProducerCode == "24")
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

                        }
                        if (i == 4)
                        {
                            //Rightmost timescale
                            xPos = 190 + 5 * xSize + 5 * 200;
                            orbShift = 0;
                            rtfList.Add(rtfTimeScale(xPos, left: false, orbShift: orbShift, xSize: xSize));
                        }
                    }
                }
            }

            //Timestamp
            curText = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm");
            rtfList.Add(rtfProg(FontSize: 8, Text: curText, XPos: 14500, YPos: 200, XSize: 1700, YSize: 200, Fill: false, Italic:true, Line:false));
            //Footer
            rtfList.Add(rtfFoot());
            string[] rtfArray = rtfList.ToArray();
            System.IO.File.WriteAllLines(@fullPath, rtfArray, Encoding.GetEncoding(1251));

            var wordApp = new Microsoft.Office.Interop.Word.Application();
            Document doc = wordApp.Documents.Open(@fullPath);
            doc.Activate();
            if (pdf)
            {
                fullPath = fullPath.Replace(".rtf", ".pdf");
                doc.SaveAs(@fullPath, WdSaveFormat.wdFormatPDF);
                // Clean up
                doc.Close(WdSaveOptions.wdDoNotSaveChanges);
                wordApp.Quit();
                System.Diagnostics.Process.Start(@fullPath);
            }

            if (word)
            {
                wordApp.Visible = true;
            }

            if (print)
            {
                object nullobj = Missing.Value;

                wordApp.Visible = true;
                int dialogResult = wordApp.Dialogs[WdWordDialog.wdDialogFilePrint].Show(ref nullobj);

                if (dialogResult == 1)
                {
                    doc.PrintOut(ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                 ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                 ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                 ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                 ref nullobj, ref nullobj);
                }
            }

        }


        private static string rtfTimeScale(int xPos = 0, bool left = true, int orbShift = 0, int xSize = 0)
        {
            string curLine = "";            
            string labelText = "";
            int startHour = 5-orbShift;
            int endHour = 30-orbShift;            
            int curXPos = 0;
            int yPos = 0;
            int len=0;
            int width=5;
            bool gor = true;
            bool punktir = false;
            bool bold = false;
            int zOrder= 1000;



            //Vertical line
            if (left)
            {
                yPos = getTwipsPositionByTime(4 * 60 * 60 + 08 * 60);
                curLine += rtfLine(false, xPos, yPos, 22000, punktir, bold, width, zOrder);
                curLine += rtfLine(false, xPos + xSize, yPos, 22000, punktir, bold, width, zOrder);
            }
            else
            {
                yPos = getTwipsPositionByTime(4 * 60 * 60 + 08 * 60);                
            }
            xPos -= 200;
            

            

            for (int hHour = startHour; hHour < endHour; hHour++ )
            {
                for (int dec = 0; dec < 6; dec++ )
                {
                    yPos = getTwipsPositionByTime((hHour+orbShift) * 60 * 60 + dec*10*60);
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
                            if (labelText.Length==1) labelText = "0"+labelText;
                            
                            curLine += rtfProg(FontSize:6,Text:labelText,XPos:xPos,YPos:yPos,XSize:200, YSize:200,Line:false,Bold:true);

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

        private static string rtfLine(bool gor, int xPos, int yPos, int len, bool punktir, bool Bold = false, int width=5, int zOrder = 1000)
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
            curLine += "{\\rtf1\\ansi \\deff4\\deflang1033{\\fonttbl{\\f4\\froman\\fcharset204";            
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




        private static string rtfFoot()
        {
            string curLine = "";
            //Foot
            curLine += "\\par }";            
            curLine += "\n";            

            return curLine;
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


        private static string rtfProg(int Inside=10, int FontSize=8, string Text = "Проверка", int XPos=1000, int YPos=1000, int XSize=2000, int YSize=2000, bool Line = true, bool Bold = false,  bool Fill=false, string Format = "", bool Italic=false, int dodhgt = 1)
        {
            string progLine = "";

            progLine += "{\\*\\do\\dobxmargin\\dobymargin\\dodhgt";
            progLine += dodhgt.ToString();
            progLine += "\\dolock\\dptxbx\\dptxbxmar";
            //nInsideN - Internal margin of the text box.
            progLine += Inside.ToString();
            progLine += "{\\dptxbxtext \\pard\\plain \\qc \\f5\\cf1\\lang1049{\\fs";
            progLine += (FontSize*2).ToString();
            if (Bold) progLine += "\\b";
            if (Italic) progLine += "\\i";
            progLine += " ";
            progLine += Text;
            progLine += "\n";
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
                progLine += "\\dpfillbggray20\\dpfillpat1";
            }
            else
            {
                progLine += "\\dpfillbgcr0\\dpfillbgcg0\\dpfillbgcb0\\dpfillpat0";
            }
            progLine += "}";
            progLine += "\n";

            return progLine;
        }



        public static void Init()
        {
            wc.Credentials = new System.Net.NetworkCredential("mike", "123");
            try
            {
                wc.Url = "http://plan12r/plan1cw/ws/ws1.1cws";                
                tvWeeks = wc.GetWeeks();                
                selectedID = getWeekInWork(tvWeeks);
            }
            catch
            {
                wc.Url = "http://tsurface/plan1cw/ws/ws1.1cws";
                tvWeeks = wc.GetWeeks();
                selectedID = getWeekInWork(tvWeeks);
                curCatConnection = "TSurfaceCatConnection";
            }
            //printReport();
            tvWeeks.Reverse();
        }


        public static SelectList getVariantsSelectList(DateTime dt, int chCode)
        {

            TVDayVariantType[] curDayVariants = wc.GetDayVariants(dt, chCode);
            

            if (curDayVariants.Length > 0)
            {
                string[] curDayVariantsArray = new string[curDayVariants.Length];
                for (int i = 0; i < curDayVariants.Length; i++)
                {                    
                    curDayVariantsArray[i] = "Вариант " + curDayVariants[i].VariantCode.ToString();
                    
                }
                var query = new SelectList(curDayVariantsArray);                                        
                return query;
            }
            else
            {
                curDayVariants = wc.GetDayVariants(dt, chCode);
                string[] curDayVariantsArray = new string[1];
                curDayVariantsArray[0] = "Вариант 1";
                var query = new SelectList(curDayVariantsArray);
                //SelectList selectList = new SelectList(curDayVariants);
                return query;
            }
        }

        

        public static TVDayVariantT[] getTVDayVariantTArray(string[] days, string[] vars)
        {
            TVDayVariantT[] res;
            List<TVDayVariantT> varList= new List<TVDayVariantT>();
            for (int i = 0; i < days.Length; i++)
            {
                TVDayVariantT curVar = new TVDayVariantT();
                curVar.VariantNumber = Convert.ToInt32(vars[i]);
                curVar.TVDayRef = days[i];
                varList.Add(curVar);
            }
            res = varList.ToArray();

            return res;
        }

        public static int getWeekInWork(TVWeekType[] tvWeeks)
        {
            int curWeekId = 0;
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - DateTime.Now.Date <= TimeSpan.FromDays(13))
                {
                    curWeekId = i;
                    break;
                }
            }

            return curWeekId;
        }
        public static int getCurrentWeek(TVWeekType[] tvWeeks)
        {
            int curWeekId = 0;
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - DateTime.Now.Date <= TimeSpan.FromDays(0))
                {
                    curWeekId = i;
                    break;
                }
            }

            return curWeekId;
        }

        public static int getWeekNumByDate(DateTime curDate)
        {
            int curWeekId = 0;
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - curDate.Date <= TimeSpan.FromDays(0))
                {
                    curWeekId = tvWeeks.Length-i-1;
                    break;
                }
            }

            return curWeekId;
        }

        public static int getWeekNumByDate(DateTime curDate)
        {
            int curWeekId = 0;
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - curDate.Date <= TimeSpan.FromDays(0))
                {
                    curWeekId = tvWeeks.Length - i - 1;
                    break;
                }
            }

            return curWeekId;
        }

        public static string getWeekRefByDate(DateTime curDate)
        {
            string weekRef = "";
            for (int i = 0; i < tvWeeks.Length; i++)
            {
                if (tvWeeks[i].BegDate.Date - curDate.Date <= TimeSpan.FromDays(0))
                {
                    weekRef = tvWeeks[i].Ref;
                    break;
                }
            }

            return weekRef;
        }


        public static int getOrbNumberByChannelCode(int chCode)
        {
            int orbNum = chCode - 10;
            return orbNum;
        }

        public static int getNearestOrb(int orbNum)
        {
            int nearestOrb = 0;
            switch (orbNum)
            {
                case 1:
                    nearestOrb = 2;
                    break;
                case 2:
                    nearestOrb = 3;
                    break;
                case 3:
                    nearestOrb = 4;
                    break;
                case 4:
                    nearestOrb = 0;
                    break;
            }
            return nearestOrb;
        }

        public static Day getDayByDateAndVariantCode(DateTime curDate, int curVar, int chCode = 10)
        {
            Day curDay = new Day();

            curDay.KanalKod = chCode;
            curDay.VariantKod = curVar;
            curDay.TVDate = curDate;
            

            TVDayVariantType[] v = wc.GetDayVariants(curDate, chCode);
            if (v.Count() > 0)
            {
                curDay.Efirs = wc.GetEfirs(curDate, chCode, curVar);

                TVDayVariantParam curParam = wc.GetVarTVDayParam(curDate, chCode, curVar);
                curDay.TVDayRef = curParam.TVDayRef;
                if (curDay.TVDayRef.Length == 0)
                {
                    curDay.TVDayRef = "dummyRef";
                    curDay.TVDayRef += curDate.Date.ToString("yyyyMMdd");
                    curDay.TVDayRef += "var";
                    curDay.TVDayRef += curVar.ToString();
                }
                curDay.Cap = curParam.Cap;
                curDay.Foot = curParam.Foot;
                curDay.FullCap += curDay.Cap;
                if (curDay.FullCap.Length > 0)
                {
                    curDay.FullCap += "\n";
                }
                curDay.FullCap += curParam.MemoryDates;
            
            }
            else
            {
                curDay.TVDayRef = "dummyRef";
                curDay.TVDayRef += curDate.Date.ToString("yyyyMMdd");
                curDay.TVDayRef += "var";
                curDay.TVDayRef += curVar.ToString();
            }
            return curDay;
        }

        public static Efir getRTA(int timing, ITCType[] ITCs)
        {
            Efir newEfir = new Efir();
            newEfir.Timing = timing;
            //Получаем общий хронометраж рекламы и анонсов (в секундах) + количество точек + чистый хронометраж
            int r = 0;
            int r99 = 0;
            int sr = 0;
            int t = 0;
            int t99 = 0;
            int st = 0;
            int a = 0;

            if (ITCs != null)
            {
                foreach (ITCType itc in ITCs)
                {
                    if (itc.Title == "Р")
                    {
                        r += itc.Timing;
                        t += itc.PointCount;
                    }
                    if (itc.Title == "Р99")
                    {
                        r99 += itc.Timing;
                        t99 += itc.PointCount;
                    }
                    if (itc.Title == "СР")
                    {
                        sr += itc.Timing;
                        st += itc.PointCount;
                    }
                    if (itc.Title == "А")
                    {
                        a += itc.Timing;
                    }

                }
            }
            newEfir.R = r;
            newEfir.R99 = r99;
            newEfir.Sr = sr;
            newEfir.T = t;
            newEfir.T99 = t99;
            newEfir.St = st;
            newEfir.A = a;

            if (r + r99 + sr + a == 0)
            {
                newEfir.PureDur = newEfir.Timing;
            }
            else
            {
                if (newEfir.Timing == r + r99 + sr + a)
                {
                    newEfir.PureDur = newEfir.Timing;
                }
                else
                {
                    newEfir.PureDur = newEfir.Timing - r - r99 - sr - a;
                }
            }
            return newEfir;
        }



    }

   

}
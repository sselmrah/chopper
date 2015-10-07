using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.Models;
using chopper1.ws1c;

namespace chopper1.Models
{
    public class Efir : EfirType
    {        
        private int _initHeight;
        private int _rTotalDur;
        private int _aTotalDur;
        private DateTime _endTime;
        private int _pureDur;
        private bool _isNextDay;
        private int _r;
        private int _t;
        private int _a;
        private bool _bold;
        private bool _italic;
        private int _fontSize;
        private bool _reserv;
        private int _grayScale;
        private bool _isNews;

        public bool IsNews
        {
            get { return _isNews; }
            set { _isNews = value; }
        }

        public int GrayScale
        {
            get { return _grayScale; }
            set { _grayScale = value; }
        }

        public bool Reserv
        {
            get { return _reserv; }
            set { _reserv = value; }
        }
        


        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        public bool Italic
        {
            get { return _italic; }
            set { _italic = value; }
        }

        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; }
        }


        public int A
        {
            get { return _a; }
            set { 
                    _a = value;
                    aToString(_a);
                }
        }

        public int T
        {
            get { return _t; }
            set { _t = value; }
        }

        public int R
        {
            get { return _r; }
            set { 
                    _r = value;
                    rToString(_r);
                }
        }

        public bool IsNextDay
        {
            get { return _isNextDay; }
            set { _isNextDay = value; }
        }

        public int PureDur
        {
            get { return _pureDur; }
            set { _pureDur = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public int ATotalDur
        {
            get { return _aTotalDur; }
            set { _aTotalDur = value; }
        }

        public int RTotalDur
        {
            get { return _rTotalDur; }
            set { _rTotalDur = value; }
        }

        public int InitHeight
        {
            get { return _initHeight; }
            set { _initHeight = value; }
        }


        public void getRTA(ITCType[] ITCs)
        {
            //Получаем общий хронометраж рекламы и анонсов (в секундах) + количество точек + чистый хронометраж
            int r = 0;
            int t = 0;
            int a = 0;

            foreach (ITCType itc in ITCs)
            {
                if (itc.Title == "Р" || itc.Title == "Р99" || itc.Title == "СР")
                {
                    r += itc.Timing;                                        
                }
                if (itc.Title == "А")
                {
                    a += itc.Timing;
                }
                t += itc.PointCount;
            }
            this.R = r;
            this.T = t;
            this.A = a;
            this.PureDur = this.Timing - r - a;
        }

        public string getInfoString()
        {
            //Получаем строку с тех. информацией (все, что внутри квадратных скобок)
            string infoString= "";
            infoString += this.timingToString(this.PureDur);
            if (this.R>0)
            {
                infoString += rToString(this.R);
                infoString += "(" + this.T.ToString() + ")";
            }
            if (this.A>0)
            {
                infoString += aToString(this.A);
            }


            return infoString;
        }

        public string getANR()
        {
            string newTitle = "";
            

            return newTitle;
        }

        public string timingToString(int pureDur)
        {
            string pureDurStr = "";
            if (pureDur > 60 * 60)
            {
                pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"h\:mm");
            }
            else
            {
                pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"mm");
            }
            return pureDurStr;
        }

        private string rToString(int rDur)
        {
            string rDurStr;
            if (rDur > 0)
            {
                if (rDur % 60 > 0)
                {
                    rDurStr = " + " + TimeSpan.FromSeconds(rDur).ToString(@"%m\:%s") + "Р";
                }
                else
                {
                    rDurStr = " + " + TimeSpan.FromSeconds(rDur).ToString(@"%m") + "Р";
                }
            }
            else
            {
                rDurStr = "";
            }
            return rDurStr;
            
        }
        private string aToString(int aDur)
        {
            string aDurStr;
            if (aDur > 0)
            {
                if (aDur % 60 > 0)
                {
                    aDurStr = " + " + TimeSpan.FromSeconds(aDur).ToString(@"%m\:%s") + "А";
                }
                else
                {
                    aDurStr = " + " + TimeSpan.FromSeconds(aDur).ToString(@"%m") + "А";
                }
            }
            else
            {
                aDurStr = "";
            }
            return aDurStr;
        }

    }
}
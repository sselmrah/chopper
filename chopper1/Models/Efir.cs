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
        private bool _isPrevDay;
        private bool _isNextDay;
        private int _r;
        private int _r99;        
        private int _sr;        
        private int _t;
        private int _t99;
        private int _st;
        private int _a;
        private bool _bold;
        private bool _italic;
        private int _fontSize;
        private bool _reserv;
        private int _grayScale;
        private bool _isNews;
        private int _nakladka;
        private int _chCode;
        private bool _isHighlighted;
        private bool _useTitle;
        private string _ageCat;
        private decimal _dSti;
        private decimal _dM;
        private decimal _dR;
        private decimal _rM;
        private decimal _rR;

        public decimal RR
        {
            get { return _rR; }
            set { _rR = value; }
        }

        public decimal RM
        {
            get { return _rM; }
            set { _rM = value; }
        }

        public decimal DR
        {
            get { return _dR; }
            set { _dR = value; }
        }

        public decimal DM
        {
            get { return _dM; }
            set { _dM = value; }
        }

        public decimal DSti
        {
            get { return _dSti; }
            set { _dSti = value; }
        }

        public string AgeCat
        {
            get { return _ageCat; }
            set { _ageCat = value; }
        }
                

        //Свойства для рисования орбитной раскладки
        //Нужно посмотреть, не сделать ли все это по-другому
        private bool _orbCh1;
        private bool _orb1;
        private bool _orb2;
        private bool _orb3;
        private bool _orb4;

        public bool Orb4
        {
            get { return _orb4; }
            set { _orb4 = value; }
        }

        public bool Orb3
        {
            get { return _orb3; }
            set { _orb3 = value; }
        }

        public bool Orb2
        {
            get { return _orb2; }
            set { _orb2 = value; }
        }

        public bool Orb1
        {
            get { return _orb1; }
            set { _orb1 = value; }
        }

        public bool OrbCh1
        {
            get { return _orbCh1; }
            set { _orbCh1 = value; }
        }

        public bool UseTitle
        {
            get { return _useTitle; }
            set { _useTitle = value; }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set { _isHighlighted = value; }
        }

        public int ChCode
        {
            get { return _chCode; }
            set { _chCode = value; }
        }
        

        public bool IsPrevDay
        {
            get { return _isPrevDay; }
            set { _isPrevDay = value; }
        }

        

        public int Nakladka
        {
            get { return _nakladka; }
            set { _nakladka = value; }
        }

        public int St
        {
            get { return _st; }
            set { _st = value; }
        }

        public int T99
        {
            get { return _t99; }
            set { _t99 = value; }
        }

        public int R99
        {
            get { return _r99; }
            set { _r99 = value; }
        }
        public int Sr
        {
            get { return _sr; }
            set { _sr = value; }
        }

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
                    rToString(_r, "Р");
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
            this.R = r;
            this.R99 = r99;
            this.Sr = sr;
            this.T = t;
            this.T99 = t99;
            this.St = st;
            this.A = a;
            
            if (r + r99 + sr + a == 0)
            {
                this.PureDur = this.Timing;
            }
            else
            {
                if (this.Timing == r + r99 + sr + a)
                {
                    this.PureDur = this.Timing;
                }
                else
                {
                    this.PureDur = this.Timing - r - r99 - sr - a;
                }
            }
        }

        public string getInfoString(int reportType = 0)
        {
            string hourDelim = "\\:";
            string minDelim = "\\:";
            //Большие столбы
            if (reportType == 1)
            {
                hourDelim = "\\.";
                minDelim = "\\'";
            }
            //Получаем строку с тех. информацией (все, что внутри квадратных скобок)
            string infoString= "";
            infoString += this.timingToString(this.PureDur, hourDelim, minDelim);
            if (this.R>0)
            {
                infoString += rToString(this.R,"Р", hourDelim, minDelim);
                infoString += "(" + this.T.ToString() + ")";
            }
            if (this.R99>0)
            {
                infoString += rToString(this.R99, "Р99", hourDelim, minDelim);
                infoString += "(" + this.T99.ToString() + ")";
            }
            if (this.Sr > 0)
            {
                infoString += rToString(this.Sr, "СР", hourDelim, minDelim);
                infoString += "(" + this.Sr.ToString() + ")";
            }
            if (this.A>0)
            {
                infoString += aToString(this.A, hourDelim, minDelim);
            }


            return infoString;
        }

        public string getANR()
        {
            string newTitle = "";
            

            return newTitle;
        }

        public string timingToString(int pureDur, string delimHour = "\\:", string delimMin = "\\:")
        {
            string pureDurStr = "";            
            if (pureDur >= 60 * 60)
            {
                if (pureDur%60>0)
                {
                    //pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"h\:mm\:ss");
                    pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"h"+@delimHour+@"mm"+@delimMin+@"ss");
                }
                else
                {
                    //pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"h"+@"\:mm");
                    pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"h" + @delimHour + @"mm");
                }
            }
            else
            {                
                if (pureDur % 60 > 0)
                {
                    //pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"mm\:ss");
                    pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"mm" + @delimMin + @"ss");
                }
                else
                {
                    pureDurStr = TimeSpan.FromSeconds(pureDur).ToString(@"mm");
                }                                
            }
            return pureDurStr;
        }

        private string rToString(int rDur, string rType, string delimHour = "\\:", string delimMin = "\\:")
        {
            string rDurStr;
            if (rDur > 0)
            {
                if (rDur % 60 > 0)
                {
                    rDurStr = " + " + TimeSpan.FromSeconds(rDur).ToString(@"%m"+delimMin+@"ss") + rType;
                }
                else
                {
                    rDurStr = " + " + TimeSpan.FromSeconds(rDur).ToString(@"%m") + rType;
                }
            }
            else
            {
                rDurStr = "";
            }
            return rDurStr;
            
        }
        private string aToString(int aDur, string delimHour = "\\:", string delimMin = "\\:")
        {
            string aDurStr;
            if (aDur > 0)
            {
                if (aDur % 60 > 0)
                {
                    //aDurStr = " + " + TimeSpan.FromSeconds(aDur).ToString(@"%m\:%s") + "А";
                    aDurStr = " + " + TimeSpan.FromSeconds(aDur).ToString(@"%m"+@delimMin+@"%s") + "А";
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
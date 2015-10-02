using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;

namespace chopper1.Models
{
    public class Day : WeekTVDayType 
    {
        //Эфиры дня
        private EfirType[] _efirs;
        private string _doWRus;

        public string DoWRus
        {
            get { return _doWRus; }
            set { _doWRus = value; }
        }

        public EfirType[] Efirs
        {
            get { return _efirs; }
            set { _efirs = value; }
        }       

    }
}
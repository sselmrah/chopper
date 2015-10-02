using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;

namespace chopper1.Models
{
    public class Week : TVWeekType
    {
        private int _daysCount;
        private WeekTVDayType[] _days;        

        public WeekTVDayType[] Days
        {
            get { return _days; }
            set { _days = value; }
        }
        public int DaysCount
        {
            get { return _daysCount; }
            set { _daysCount = value; }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;

namespace chopper1.Models
{
    public class Week : TVWeekType
    {
        private int _daysCount;
        private List<WeekTVDayType> _days;
        private int[] _availableChannels;

        public int[] AvailableChannels
        {
            get { return _availableChannels; }
            set { _availableChannels = value; }
        }
                

        public List<WeekTVDayType> Days
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
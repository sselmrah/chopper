using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace chopper1.Models
{
    public class advSearch
    {
        private string _title;
        private DateTime _timingMin;
        private DateTime _timingMax;
        private bool _bitRepetition;
        private bool _bitOriginal;
        private DateTime _dateMin;
        private DateTime _dateMax;        
        private bool _monday;
        private bool _tuesday;
        private bool _wednesday;
        private bool _thursday;
        private bool _friday;
        private bool _saturday;
        private bool _sunday;
        private double _timeStartMinDbl;
        private double _timeStartMaxDbl;
        private double _dStiMin;
        private double _dStiMax;
        private double _dMMin;
        private double _dMMax;
        private double _dRMin;
        private double _dRMax;
        private int[] _producerCode;
        private int[] _sellerCode;
        private DateTime _timeStartMin;
        private DateTime _timeStartMax;
        private string[] _producers;

        public bool BitOriginal
        {
            get { return _bitOriginal; }
            set { _bitOriginal = value; }
        }

        public string[] Producers
        {
            get { return _producers; }
            set { _producers = value; }
        }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TimeStartMax
        {
            get { return _timeStartMax; }
            set { _timeStartMax = value; }
        }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TimeStartMin
        {
            get { return _timeStartMin; }
            set { _timeStartMin = value; }
        }


        public int[] SellerCode
        {
            get { return _sellerCode; }
            set { _sellerCode = value; }
        }

        public int[] ProducerCode
        {
            get { return _producerCode; }
            set { _producerCode = value; }
        }

        public double DRMax
        {
            get { return _dRMax; }
            set { _dRMax = value; }
        }

        public double DRMin
        {
            get { return _dRMin; }
            set { _dRMin = value; }
        }

        public double DMMax
        {
            get { return _dMMax; }
            set { _dMMax = value; }
        }

        public double DMMin
        {
            get { return _dMMin; }
            set { _dMMin = value; }
        }

        public double DStiMax
        {
            get { return _dStiMax; }
            set { _dStiMax = value; }
        }

        public double DStiMin
        {
            get { return _dStiMin; }
            set { _dStiMin = value; }
        }

        public double TimeStartMaxDbl
        {
            get { return _timeStartMaxDbl; }
            set { _timeStartMaxDbl = value; }
        }

        public double TimeStartMinDbl
        {
            get { return _timeStartMinDbl; }
            set { _timeStartMinDbl = value; }
        }

        public bool Sunday
        {
            get { return _sunday; }
            set { _sunday = value; }
        }

        public bool Saturday
        {
            get { return _saturday; }
            set { _saturday = value; }
        }

        public bool Friday
        {
            get { return _friday; }
            set { _friday = value; }
        }

        public bool Thursday
        {
            get { return _thursday; }
            set { _thursday = value; }
        }

        public bool Wednesday
        {
            get { return _wednesday; }
            set { _wednesday = value; }
        }

        public bool Tuesday
        {
            get { return _tuesday; }
            set { _tuesday = value; }
        }

        public bool Monday
        {
            get { return _monday; }
            set { _monday = value; }
        }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateMax
        {
            get { return _dateMax; }
            set { _dateMax = value; }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateMin
        {
            get { return _dateMin; }
            set { _dateMin = value; }
        }

        public bool BitRepetition
        {
            get { return _bitRepetition; }
            set { _bitRepetition = value; }
        }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TimingMin
        {
            get { return _timingMin; }
            set { _timingMin = value; }
        }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TimingMax
        {
            get { return _timingMax; }
            set { _timingMax = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }        

    }
}
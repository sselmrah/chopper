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
        private string _fullCap;
        private TVDayVariantType[] _chOneVariants;
        private TVDayVariantType[] _orb1Variants;
        private TVDayVariantType[] _orb2Variants;
        private TVDayVariantType[] _orb3Variants;
        private TVDayVariantType[] _orb4Variants;
        private DateTime _renderTime;


        public string FullCap
        {
            get { return _fullCap; }
            set { _fullCap = value; }
        }
        public DateTime RenderTime
        {
            get { return _renderTime; }
            set { _renderTime = value; }
        }

        public TVDayVariantType[] Orb4Variants
        {
            get { return _orb4Variants; }
            set { _orb4Variants = value; }
        }

        public TVDayVariantType[] Orb3Variants
        {
            get { return _orb3Variants; }
            set { _orb3Variants = value; }
        }

        public TVDayVariantType[] Orb2Variants
        {
            get { return _orb2Variants; }
            set { _orb2Variants = value; }
        }

        public TVDayVariantType[] Orb1Variants
        {
            get { return _orb1Variants; }
            set { _orb1Variants = value; }
        }

        public TVDayVariantType[] ChOneVariants
        {
            get { return _chOneVariants; }
            set { _chOneVariants = value; }
        }

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
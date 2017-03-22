using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chopper1
{
    public static class StringExtensions
    {
        public static string Left(this string s, int left)
        {
            if (s.Length >= left & left>0)
            {
                return s.Substring(0, left);
            }
            else
            {
                return s;
            }

        }
        public static string Right(this string s, int right)
        {
            if (s.Length >= right & right > 0)
            {
                return s.Substring(s.Length - right, right);
            }
            else
            {
                return s;
            }

        }
        public static string EngToRus(this string engTitle)
        {
            string rusTitle = "";
            engTitle = engTitle.Replace('q', 'й').Replace('w', 'ц').Replace('e', 'у').Replace('r', 'к').Replace('t', 'е').Replace('y', 'н').Replace('u', 'г').Replace('i', 'ш').Replace('o', 'щ').Replace('p', 'з').Replace('[', 'х').Replace(']', 'ъ'); 
            engTitle = engTitle.Replace('a', 'ф').Replace('s', 'ы').Replace('d', 'в').Replace('f', 'а').Replace('g', 'п').Replace('h', 'р').Replace('j', 'о').Replace('k', 'л').Replace('l', 'д').Replace(';', 'ж').Replace('\'', 'э'); 
            engTitle = engTitle.Replace('z', 'я').Replace('x', 'ч').Replace('c', 'с').Replace('v', 'м').Replace('b', 'и').Replace('n', 'т').Replace('m', 'ь').Replace(',', 'б').Replace('.', 'ю'); 
            rusTitle = engTitle;
            return rusTitle;
        }

    }
}
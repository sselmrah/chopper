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
            if (s.Length >= left)
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
            if (s.Length >= right)
            {
                return s.Substring(s.Length - right, right);
            }
            else
            {
                return s;
            }

        }
    }
}
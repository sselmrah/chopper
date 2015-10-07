using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chopper1.ws1c;

namespace chopper1
{
    public class MyStartupClass
    {
        public static WebСервис1 wc = new WebСервис1();
        public static void Init()
        {
            wc.Credentials = new System.Net.NetworkCredential("mike", "123");
        }

    }

}
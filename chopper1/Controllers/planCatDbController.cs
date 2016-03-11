using chopper1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace chopper1.Controllers
{
    public class planCatDbController : Controller
    {
        // GET: planCatalogueDb

        public planCatDb planDb = new planCatDb();


        public ActionResult Index()
        {
            bool i = planDb.Open();
            /*
            if (i)
            {

                return Content(i.ToString() + "\n" + planDb.getDataTest("")+ "\n");
            }
            else
            {
                return Content(i.ToString());
            }
            */
            DataTable dt = planDb.getDataTest2("");

            return View(dt);
        }

        [HttpPost]
        public ActionResult Search(string title="заговор диетологов")
        {
            title = Request.Form["catSearchTb"].ToString();
            bool i = planDb.Open();
            DataTable dt = planDb.getDataTest2(title);
            return View(dt);
        }
    }

 

   
}
using chopper1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chopper1.ws1c;

namespace chopper1.Controllers
{
    public class UserViewController : Controller
    {
        ApplicationDbContext context;
        UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        WebСервис1 curWc = MyStartupClass.wc;        
        public UserViewController ()
        {
            context = new ApplicationDbContext();
        }
        // GET: UserView
        public ActionResult Index()
        {
            var allUsers = context.Users.ToList();            
            
            var userVM = allUsers.Select(user => new UserViewModel { Username = user.UserName, Roles = userManager.GetRoles(user.Id).ToList() }).ToList();
            List<UserViewModel> userVM2 = new List<UserViewModel>();
            var model = new GroupedUserViewModel { Users = userVM };

            if (context.Departments.Count() == 0)
            {
                Department newDep = new Department();
                newDep.Code = 1;
                newDep.Director = "Титинков Сергей Алексеевич";
                newDep.Name = "Дирекция кинопоказа";
                var departments = context.Departments.Add(newDep);
            }
            
            return View(model);
        }

        public ActionResult Delete(string userName)
        {
            var thisUser = context.Users.Where(r => r.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Users.Remove(thisUser);
            context.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult ViewRightsByUser(string userName = "Ковалев")
        {
            DateTime today = DateTime.Now;
            List<Tuple<DateTime,int, ChopperRightsT>> rightsList = new List<Tuple<DateTime,int, ChopperRightsT>>();
            int chCode = 10;
            int varCode = 1;
            for (int i=-10; i<10; i++)
            {
                
                DateTime curDate = today+TimeSpan.FromDays(i);
                TVDayVariantType[] vars = curWc.GetDayVariants(curDate, chCode);
                if (vars != null)
                {
                    foreach (TVDayVariantType v in vars)
                    {
                        varCode = v.VariantCode;
                        ChopperRightsT curRights = curWc.GetUserRights(chCode, curDate.Date, varCode, userName);
                        if (curRights == null)
                        {
                            curRights = new ChopperRightsT();
                            curRights.add = false;
                            curRights.del = false;
                            curRights.read = false;
                            curRights.write = false;
                            curRights.filter = "";
                        }
                        Tuple<DateTime, int, ChopperRightsT> curTuple = Tuple.Create(curDate, varCode, curRights);
                        rightsList.Add(curTuple);
                    }
                }
            }
            ViewBag.userName = userName;
            

            return View(rightsList);
        }

        [HttpPost]
        public ActionResult SwitchRights(string rType = "read", string tvDate = "01.01.2017", string varNum = "1", string curValue = "0", string userName = "Somebody")
        {
            DateTime curDt = DateTime.Parse(tvDate).Date;
            int chCode = 10;
            int varCode = Convert.ToInt32(varNum);

            //ChopperRightsT curRights = curWc.GetUserRights(curDt,chCode,varCode,userName);
            ChopperRightsT curRights = curWc.GetUserRights(chCode, curDt, varCode, userName);
            switch (rType)
                {
                    case "read":
                        curRights.read = !curRights.read;
                        break;
                    case "write":
                        curRights.write = !curRights.write;
                        break;
                    case "add":
                        curRights.add = !curRights.add;
                        break;
                    case "del":
                        curRights.del = !curRights.del;
                        break;
                }

            //curWc.GetDayVarUsersRights - права по дню/варианту

            //curWc.SetUserRights(curDt, chCode, varCode, userName, curRights);
            curWc.SetUserRights(chCode, curDt, varCode, userName, curRights, false);
            curRights = curWc.GetUserRights(chCode, curDt, varCode, userName);       
            return View();
        }

    }
}
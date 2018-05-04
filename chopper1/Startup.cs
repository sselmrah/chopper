using Microsoft.Owin;
using Owin;
using chopper1.ws1c;
using chopper1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;


[assembly: OwinStartupAttribute(typeof(chopper1.Startup))]
namespace chopper1
{    
    public partial class Startup
    {
        public static ApplicationDbContext context = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            createRolesandUsers();
            createBaseSettings();
        }

        private void createBaseSettings()
        {
            Settings curSet = new Settings();
            curSet.UserName = "Global";
            var globalSettingsExist = context.Settings.Any(x => x.UserName == curSet.UserName);
            if (!globalSettingsExist)
            {
                curSet.Green1 = "218,226,170";
                curSet.Green2 = "123,193,106";
                curSet.Green3 = "1,140,73";
                curSet.Red1 = "241,175,173";
                curSet.Red2 = "238,90,96";
                curSet.Red3 = "235,49,48";

                curSet.BaseShare = 18;
                curSet.StepShare = 3;
                context.Settings.Add(curSet);
                context.SaveChanges();
            }
            curSet.UserName = "Default";
            var defaultSettingsExist = context.Settings.Any(x => x.UserName == curSet.UserName);
            if (!defaultSettingsExist)
            {
                curSet.Green1 = "218,226,170";
                curSet.Green2 = "123,193,106";
                curSet.Green3 = "1,140,73";
                curSet.Red1 = "241,175,173";
                curSet.Red2 = "238,90,96";
                curSet.Red3 = "235,49,48";

                curSet.BaseShare = 18;
                curSet.StepShare = 3;
                context.Settings.Add(curSet);
                context.SaveChanges();
            }

                
        }

        // In this method we create default User roles and Admin user for login   
        private void createRolesandUsers()
        {


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "amosendz";
                user.Email = "amosendz@gmail.com";

                string userPWD = "234235236";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Employee"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Employee";
                roleManager.Create(role);

            }
        } 
    }
}

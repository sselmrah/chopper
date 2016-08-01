namespace chopper1.Migrations
{
    using chopper1.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<chopper1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "chopper1.Models.ApplicationDbContext";
        }

        protected override void Seed(chopper1.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            /*
            var hasher = new PasswordHasher();
            context.Users.AddOrUpdate(u => u.UserName,
                new ApplicationUser { UserName = "amosendz", PasswordHash = hasher.HashPassword("123") }
                );
            */


            if (context.Users.Any(u => u.UserName == "amosendz"))
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = new ApplicationUser { UserName = "amosendz" };
                
                userManager.Create(user, "123");
                roleManager.Create(new IdentityRole { Name = "admin" });
                userManager.AddToRole(user.Id, "admin");
            }

        }
    }
}

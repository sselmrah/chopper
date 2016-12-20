using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace chopper1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /*
        private string _favoriteBook;

        public string FavoriteBook
        {
            get { return _favoriteBook; }
            set { _favoriteBook = value; }
        }
        */



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }





    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<DayAccess> DaysAccess { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }

    public class Department
    {
        private int _id;
        private string _name;
        private int _code;
        private string _director;

        public string Director
        {
          get { return _director; }
          set { _director = value; }
        }

        public int Code
        {
          get { return _code; }
          set { _code = value; }
        }

        public string Name
        {
          get { return _name; }
          set { _name = value; }
        }

        public int Id
        {
          get { return _id; }
          set { _id = value; }
        }
    }

    public class DayAccess
    {
        private int _id;

        
        private string _tvDayRef;
        private string _permitted;
        private string _semiPermitted;
        private string _forbidden;
        
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        public string TvDayRef
        {
            get { return _tvDayRef; }
            set { _tvDayRef = value; }
        }
        
        public string Permitted
        {
            get { return _permitted; }
            set { _permitted = value; }
        }
        
        public string SemiPermitted
        {
            get { return _semiPermitted; }
            set { _semiPermitted = value; }
        }
        

        public string Forbidden
        {
            get { return _forbidden; }
            set { _forbidden = value; }
        }


    }

}
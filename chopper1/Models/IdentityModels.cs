using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace chopper1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required] //data annotations allow me to define information,         
        [Display(Name = "Код дирекции")] //and also the shape field in the table
        public int departmentCode { get; set; }
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
        public DbSet<Settings> Settings { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }

    public class Settings
    {
        private int _id;
        private string _red1;
        private string _red2;
        private string _red3;
        private string _green1;
        private string _green2;
        private string _green3;
        private decimal _baseShare;
        private decimal _stepShare;
        private string _userName;


        public string Red1 { get => _red1; set => _red1 = value; }
        public string Red2 { get => _red2; set => _red2 = value; }
        public string Red3 { get => _red3; set => _red3 = value; }
        public string Green1 { get => _green1; set => _green1 = value; }
        public string Green2 { get => _green2; set => _green2 = value; }
        public string Green3 { get => _green3; set => _green3 = value; }
        public decimal BaseShare { get => _baseShare; set => _baseShare = value; }
        public decimal StepShare { get => _stepShare; set => _stepShare = value; }
        public string UserName { get => _userName; set => _userName = value; }
        public int Id { get => _id; set => _id = value; }
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
        private int _accessLevel;

        public int AccessLevel
        {
            get { return _accessLevel; }
            set { _accessLevel = value; }
        }

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
        

    }

}
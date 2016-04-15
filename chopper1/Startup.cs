using Microsoft.Owin;
using Owin;
using chopper1.ws1c;


[assembly: OwinStartupAttribute(typeof(chopper1.Startup))]
namespace chopper1
{    
    public partial class Startup
    {        
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASP.MVC.Scratch.App_Start.Startup))]
namespace ASP.MVC.Scratch.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

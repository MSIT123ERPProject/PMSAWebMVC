using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PMSAWebMVC.Startup))]
namespace PMSAWebMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

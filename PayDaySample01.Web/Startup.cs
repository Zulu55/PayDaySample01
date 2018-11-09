using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PayDaySample01.Web.Startup))]
namespace PayDaySample01.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

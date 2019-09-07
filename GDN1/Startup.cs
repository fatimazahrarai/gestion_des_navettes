using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GDN1.Startup))]
namespace GDN1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

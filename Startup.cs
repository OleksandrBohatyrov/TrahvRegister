using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrajvRegister10.Startup))]
namespace TrajvRegister10
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

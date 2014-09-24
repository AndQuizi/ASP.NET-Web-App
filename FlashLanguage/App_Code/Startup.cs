using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(andrew.Startup))]
namespace andrew
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

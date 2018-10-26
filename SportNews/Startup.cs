using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SportNews.Startup))]
namespace SportNews
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

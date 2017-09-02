using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Products1.Backend.Startup))]
namespace Products1.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

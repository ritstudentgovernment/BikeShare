using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BikeShare.Startup))]

namespace BikeShare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
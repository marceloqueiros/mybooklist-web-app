using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyBookList.Startup))]
namespace MyBookList
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

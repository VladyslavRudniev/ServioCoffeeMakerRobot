using System.Web.Http;
using Owin;

namespace WebAPI.OWIN
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Use(typeof(LoggerModule), "Logger: ");
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "{controller}");
            app.UseWebApi(config);
        }
    }
}
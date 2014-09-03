using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using FluentScheduler;

namespace BikeShare
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteConfig.RegisterRoutes(routes);
        }

        /// <summary>
        /// Runs at application startup
        /// Adds App administrators to the database if not present already
        /// </summary>
        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BikeShare.App_Start.BootstrapBundleConfig.RegisterBundles();
            TaskManager.Initialize(new BikeShare.Code.MailerRegistry()); 
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //Database.SetInitializer<BikeShare.Models.BikesContext>(new BikeShare.Models.BikesContext.SettingsInitializer());
        }
    }
}
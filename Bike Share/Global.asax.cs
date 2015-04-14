using FluentScheduler;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
            TaskManager.Initialize(new BikeShare.Code.MailerRegistry());
        }
    }
}
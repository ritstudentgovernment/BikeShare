using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;

namespace BikeShare
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //Smile & Frown
            routes.MapRoute(name: "s", url: "s/{pointId}", defaults: new {controller = "smile", action = "smile", pointId = new RangeRouteConstraint(1, 999999)});
            routes.MapRoute(name: "f", url: "f/{pointId}", defaults: new { controller = "smile", action = "frown", pointId = new RangeRouteConstraint(1, 999999)});
            //Explore

            //Super

            //Admin
            
            //Mailing Unsubscription
            routes.MapRoute(name: "MailSubFail", url: "Mail/Unsubscribe/Failed", defaults: new { controller = "MailSub", action = "failure" });
            routes.MapRoute(name: "MailSub", url: "Mail/Unsubscribe/{subId}", defaults: new { controller = "MailSub", action = "unsub", subId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "MailSubSucc", url: "Mail/Unsubscribed", defaults: new { controller = "MailSub", action = "success" });

            //Account logon and logoff
            routes.MapRoute(name: "logon", url: "LogOn/{returnUrl}/", defaults: new { controller = "Account", action = "LogOn", returnUrl = "" });
            routes.MapRoute(name: "logoff", url: "LogOff/", defaults: new { controller = "Account", action = "LogOff" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo*}");
           
        }
    }
}
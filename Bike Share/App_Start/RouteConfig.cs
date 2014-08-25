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
            //Home
            routes.MapRoute(name: "siteAbout", url: "About", defaults: new { controller = "Home", action = "About" });
            //Explore
            routes.MapRoute(name: "ExploreHome", url: "Me", defaults: new { controller = "Explore", action = "Index" });
            routes.MapRoute(name: "ExploreBikes", url: "Bikes", defaults: new { controller = "Explore", action = "bikeListing" });
            routes.MapRoute(name: "ExploreRacks", url: "Racks", defaults: new { controller = "Explore", action = "rackListing"});
            routes.MapRoute(name: "ExploreRack", url: "Racks/{rackId}", defaults: new { controller = "Explore", action = "rackDetails", rackId = new RangeRouteConstraint(1, 999999) });
            //Checkout
            routes.MapRoute(name: "checkoutSelect", url: "Cashier", defaults: new { controller = "Checkout", action = "SelectRack" });
            routes.MapRoute(name: "checkout", url: "Cashier/{rackId}", defaults: new { controller = "Checkout", action = "Index", rackId = new RangeRouteConstraint(1, 999999) });

            //Mechanic
            routes.MapRoute(name: "MechanicHome", url: "Mechanic", defaults: new { controller = "Mechanic", action = "Index" });
            routes.MapRoute(name: "MechanicBikes", url: "Mechanic/Bikes/{pageId}", defaults: new { controller = "Mechanic", action = "bikeListing", pageId = 1 });
            routes.MapRoute(name: "MechanicBike", url: "Mechanic/Bike/{bikeId}", defaults: new { controller = "Mechanic", action = "bikeDetails", bikeId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "MechanicSpecs", url: "Mechanic/Inspections/{pageId}", defaults: new { controller = "Mechanic", action = "bikeInspections", pageId = 1 });
            routes.MapRoute(name: "MechanicSpec", url: "Mechanic/Inspection/{specId}", defaults: new { controller = "Mechanic", action = "inspectionDetails", specID = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "MechanicMaints", url: "Mechanic/Issues/{pageId}", defaults: new { controller = "Mechanic", action = "bikeMaintenance", pageId = 1 });
            routes.MapRoute(name: "MechanicMaint", url: "Mechanic/Issue/{maintId}", defaults: new { controller = "Mechanic", action = "maintenanceDetails", maintId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "MechanicProfile", url: "Mechanic/Me/{hourPage}/{activityPage}", defaults: new { controller = "Mechanic", action = "myActivity", hourPage = 1 , activityPage = 1});
            //Dashboard
            routes.MapRoute(name: "AdminHome", url: "Manage", defaults: new { controller = "Admin", action = "Index" });
            routes.MapRoute(name: "AdminSettings", url: "Manage/Settings", defaults: new { controller = "Admin", action = "appSettings" });
            routes.MapRoute(name: "AdminCheckouts", url: "Manage/Bikes/Rentals/{pageId}", defaults: new { controller = "Admin", action = "bikeCheckouts", pageId = 1 });
            routes.MapRoute(name: "AdminBikes", url: "Manage/Bikes/{pageId}", defaults: new { controller = "Admin", action = "bikeList", pageId = 1 });
            routes.MapRoute(name: "AdminNewRack", url: "Manage/Racks/New", defaults: new { controller = "Admin", action = "newRack" });
            routes.MapRoute(name: "AdminRacks", url: "Manage/Racks/{pageId}", defaults: new { controller = "Admin", action = "bikeRackList", pageId = 1 });
            routes.MapRoute(name: "AdminCharges", url: "Manage/Finance/{pageId}", defaults: new { controller = "Admin", action = "chargesList", pageId = 1 });
            routes.MapRoute(name: "AdminChargeDetails", url: "Manage/Finance/Charge/{chargeId}", defaults: new { controller = "Admin", action = "chargeDetails", chargeId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "AdminRackEdit", url: "Manage/Racks/Rack/{rackId}", defaults: new { controller = "Admin", action = "editRack", rackId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "AdminBikeDetails", url: "Manage/Bikes/Bike/{bikeId}", defaults: new { controller = "Admin", action = "infoBike", bikeId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "AdminInspectionList", url: "Manage/Bikes/Inspections/{pageId}", defaults: new { controller = "Admin", action = "inspectionList", pageId = 1});
            routes.MapRoute(name: "AdminMaintDetails", url: "Manage/Bikes/Maintenance/View/{maintId}", defaults: new { controller = "Admin", action = "maintenanceDetails", maintId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "AdminMaintList", url: "Manage/Bikes/Maintenance/{pageId}", defaults: new { controller = "Admin", action = "maintenanceList", pageId = 1});
            routes.MapRoute(name: "AdminShopsNew", url: "Manage/Workshops/New", defaults: new { controller = "Admin", action = "newWorkshop"});
            routes.MapRoute(name: "AdminUserDetails", url: "Manage/Users/View/{userId}", defaults: new { controller = "Admin", action = "userDetails", userId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "AdminUserList", url: "Manage/Users/{pageId}", defaults: new { controller = "Admin", action = "userList", pageId = 1});
            routes.MapRoute(name: "AdminWorkshopDetails", url: "Manage/Workshops/View/{shopId}", defaults: new { controller = "Admin", action = "workshopDetails", shopId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "AdminWorkshopList", url: "Manage/Workshops/{pageId}", defaults: new { controller = "Admin", action = "workshopList", pageId = 1});
            //Mailing Unsubscription
            routes.MapRoute(name: "MailSubFail", url: "Mail/Unsubscribe/Failed", defaults: new { controller = "MailSub", action = "failure" });
            routes.MapRoute(name: "MailSub", url: "Mail/Unsubscribe/{subId}", defaults: new { controller = "MailSub", action = "unsub", subId = new RangeRouteConstraint(1, 999999) });
            routes.MapRoute(name: "MailSubSucc", url: "Mail/Unsubscribed", defaults: new { controller = "MailSub", action = "success" });

            //Account logon and logoff
            //routes.MapRoute(name: "logon", url: "LogOn/{returnUrl}/", defaults: new { controller = "Account", action = "LogOn", returnUrl = "" });
            //routes.MapRoute(name: "logoff", url: "LogOff/", defaults: new { controller = "Account", action = "LogOff" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo*}");
           
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Models;
using System.Data.Entity;

namespace BikeShare.Code
{
    /// <summary>
    /// Filter for inserting userName into views. This helps to enable unit testing when the action would otherwise directly depend on the roleManager.
    /// </summary>
    public class UserNameFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Called when executing the action. Inserts the correct userName into the arguments.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string Key = "userName";

            if (filterContext.ActionParameters.ContainsKey(Key))
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.ActionParameters[Key] = filterContext.HttpContext.User.Identity.Name;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public static class roleCheck
    {
        public static bool isUserAdmin(string userName)
        {
            using (var context = new BikesContext())
            {
                return context.BikeUser.Where(u => u.userName == userName).First().canAdministerSite;
            }
        }
        public static bool isUserMechanic(string userName)
        {
            using (var context = new BikesContext())
            {
                return context.BikeUser.Where(u => u.userName == userName).First().canMaintainBikes;
            }
        }
        public static bool isUserCashier(string userName)
        {
            using (var context = new BikesContext())
            {
                return context.BikeUser.Where(u => u.userName == userName).First().canCheckOutBikes;
            }
        }
    }
}
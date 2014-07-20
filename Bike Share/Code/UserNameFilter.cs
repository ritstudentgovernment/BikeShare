using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGBikeShare.Code
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
}
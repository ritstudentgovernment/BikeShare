using System;
using System.Web.Mvc;
using System.Web.Security;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles logging on and off.
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Displays the logon form. Logs the current user off if someone is logged in.
        /// </summary>
        /// <param name="returnUrl">Url to redirect to on successful log on.</param>
        /// <returns>Static view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOn(String returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOff();
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Processes the login attempt.
        /// </summary>
        /// <param name="model">Information about the login.</param>
        /// <param name="returnUrl">Url to redirect to on successful logon.</param>
        /// <returns>View with validation errors or redirects to return url.</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOn(BikeShare.Models.Login model, String returnUrl = "")
        {
            //If in debug mode, bypasses authentication and logs in as the provided userName
#if DEBUG
            FormsAuthentication.RedirectFromLoginPage(model.UserName, model.RememberMe);
#endif
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    Redirect(returnUrl);
                }
                ModelState.AddModelError("", "Incorrect username and/or password");
            }
            return View(model);
        }

        /// <summary>
        /// Logs the current user off.
        /// </summary>
        /// <returns>Redirects to the logon page.</returns>
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn", "Account", null);
        }
    }
}
using BikeShare.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles logging on and off.
    /// </summary>
    public class AccountController : Controller
    {
        private BikesContext context;

        public AccountController()
        {
            context = new BikesContext();
        }

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
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(BikeShare.Models.Login model, String returnUrl = "")
        {
            model.UserName = model.UserName.ToLower();
            //If in debug mode, bypasses authentication and logs in as the provided userName
#if DEBUG
            if (context.BikeUser.Where(u => u.userName == model.UserName).Count() < 1)
            {
                context.BikeUser.Add(new bikeUser { userName = model.UserName, canBorrowBikes = true, isArchived = false, email = User.Identity.Name + context.settings.First().expectedEmail });
                context.SaveChanges();
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                return RedirectToAction("Register", "Explore");
            }
            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
            return RedirectToAction("Index", "Explore");
#endif
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    if (context.BikeUser.Where(u => u.userName == User.Identity.Name).Count() < 1)
                    {
                        context.BikeUser.Add(new bikeUser { userName = User.Identity.Name, canBorrowBikes = true, isArchived = false, email = User.Identity.Name + context.settings.First().expectedEmail });
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        return RedirectToAction("Register", "Explore");
                    }
                    FormsAuthentication.RedirectFromLoginPage(model.UserName, true);
                }
                ModelState.AddModelError("", "Incorrect username and/or password");
            }
            return View(model);
        }

        public ActionResult LogOnForm()
        {
            return LogOn("");
        }

        /// <summary>
        /// Logs the current user off.
        /// </summary>
        /// <returns>Redirects to the logon page.</returns>
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", null);
        }
    }
}
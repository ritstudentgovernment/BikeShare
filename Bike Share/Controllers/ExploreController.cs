using BikeShare.Models;
using BikeShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles all of the data exploration pages.
    /// </summary>
    public class ExploreController : Controller
    {
        private BikesContext context;
        private int pageSize = 25;

        /// <summary>
        /// Initializes the controller with dependency injection.
        /// </summary>
        /// <param name="param">IExploreRepository implementation to use.</param>
        public ExploreController()
        {
            context = new BikesContext();
        }

        [Authorize]
        public ActionResult Register()
        {
            ViewBag.registerHTML = context.settings.First().registerHTML;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string firstName, string lastName, string phoneNumber)
        {
            var user = context.BikeUser.Where(u => u.userName == User.Identity.Name).First();
            if (!user.isArchived && user.canBorrowBikes)
            {
                user.lastRegistered = DateTime.Now;
            }
            user.firstName = firstName;
            user.lastName = lastName;
            user.phoneNumber = phoneNumber;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns the Explore/Index page with no ViewModel.
        /// </summary>
        /// <returns>View without ViewModel.</returns>
        ///
        [Authorize]
        public ActionResult index(int page = 1)
        {
            var user = context.BikeUser.Where(u => u.userName == User.Identity.Name).First();
            if (user.isArchived || user.lastRegistered.AddDays(context.settings.First().daysBetweenRegistrations) < DateTime.Now)
            {
                return RedirectToAction("Register");
            }
            var model = new BikeShare.ViewModels.profileViewModel();
            model.user = user;
            model.pagingInfo = new PageInfo(0, 25, 1);
            model.cards = new List<ActivityCard>();
            if (context.CheckOut.Where(c => c.rider == user.bikeUserId).Where(i => !i.isResolved).Count() > 0)
            {
                model.hasRental = true;
                model.hoursLeft = (int)context.CheckOut.Where(c => c.rider == user.bikeUserId).Where(i => !i.isResolved).First().timeOut.AddHours(24).Subtract(DateTime.Now).TotalHours;
            }
            return View(model);
        }

        /// <summary>
        /// Displays a listing of all bike racks in the system.
        /// </summary>
        /// <returns></returns>
        public ActionResult rackListing(int page = 1)
        {
            var model = new PaginatedViewModel<BikeRack>();
            model.modelList = context.BikeRack.Where(a => !a.isArchived).ToList();
            DateTime dateFloor = DateTime.Now.Subtract(new TimeSpan(context.settings.First().DaysBetweenInspections));
            ViewBag.availableBikes = context.Bike.ToList().Where(b => b.isAvailable()).ToList();
            model.pagingInfo = new PageInfo(model.modelList.Count(), model.modelList.Count(), page);
            var images = new Dictionary<int, string>();
            ViewBag.images = images;
            return View(model);
        }

        /// <summary>
        /// Disposes of the controller and resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult userEdit(int userId)
        {
            return View(context.BikeUser.Find(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult userEdit(int userId, [Bind(Include = "phoneNumber,email,firstName,lastName")] bikeUser user)
        {
            var old = context.BikeUser.Find(userId);
            old.phoneNumber = user.phoneNumber;
            old.email = user.email;
            old.firstName = user.firstName;
            old.lastName = user.lastName;
            context.SaveChanges();
            return RedirectToAction("Index", "Explore");
        }

        public ActionResult rackDetails(int rackId)
        {
            var rack = context.BikeRack.Find(rackId);
            return View(rack);
        }

        private string getUrlForRackImage(int rackId)
        {
            if (System.IO.File.Exists(Request.PhysicalApplicationPath.ToString() + "\\Content\\Images\\Racks\\" + rackId + ".jpg"))
            {
                return Url.Content("~/Content/Images/Racks/" + rackId.ToString() + ".jpg");
            }
            else
            {
                return Url.Content("~/Content/Images/Racks/Default.jpg");
            }
        }
    }
}
using BikeShare.Models;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using BikeShare.ViewModels;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles all of the data exploration pages.
    /// </summary>
    public class ExploreController : Controller
    {
        BikesContext context;
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
            model.cards = new List<ActivityCard>();
            model.pagingInfo = new PageInfo(context.tracer.Where(u => u.user.bikeUserId == user.bikeUserId).Count(), pageSize, (page - 1) * pageSize);
            if (context.CheckOut.Where(c => c.user.bikeUserId == user.bikeUserId).Where(i => !i.isResolved).Count() > 0)
            {
                model.hasRental = true;
                model.hoursLeft = (int)context.CheckOut.Where(c => c.user.bikeUserId == user.bikeUserId).Where(i => !i.isResolved).First().timeOut.AddHours(24).Subtract(DateTime.Now).TotalHours;
            }
            foreach(var item in context.tracer.Where(u => u.user.bikeUserId == user.bikeUserId).OrderByDescending(d => d.time).Skip((page - 1) * pageSize).Take(pageSize).ToList())
            {
                var building = new ActivityCard { date = item.time, userName = User.Identity.Name, userId = model.user.bikeUserId };
                if (item.charge != null)
                {
                    building.title = "Charge: " + item.charge.title;
                    if (item.charge.isResolved)
                    {building.status = cardStatus.success;}
                    else
                    {building.status = cardStatus.danger;}
                    building.type = activityType.charge;
                }
                if (item.checkOut != null)
                {
                    building.title = "Checkout: " + item.checkOut.timeOut.ToShortDateString();
                    building.status = cardStatus.success;
                    building.type = activityType.checkout;
                }
                if (item.inspection != null)
                {
                    if (item.inspection.comment != null)
                    {
                        building.title = "Inspection: " + item.inspection.comment.ToString();
                    }
                    else
                    {
                        building.title = "Inspection";
                    }
                    if (item.inspection.isPassed) { building.status = cardStatus.success; } else { building.status = cardStatus.danger; }
                    building.type = activityType.inspection;
                }
                if (item.maint != null)
                {
                    building.title = "Maintenance: " + item.maint.title;
                    if (item.maint.resolved) { building.status = cardStatus.success; } else { building.status = cardStatus.danger; }
                    building.type = activityType.maintenance;
                }
                if (item.rack != null)
                {
                    building.title = "Rack: " + item.rack.name;
                    building.status = cardStatus.defaults;
                    building.type = activityType.admin;
                }
                if (item.shop != null)
                {
                    building.title = "Workshop: " + item.shop.name;
                    building.status = cardStatus.defaults;
                    building.type = activityType.admin;
                }
                if (item.update != null)
                {
                    building.title = "Comment on Maintenance: " + item.update.title;
                    building.status = cardStatus.defaults;
                    building.type = activityType.comment;
                }
                model.cards.Add(building);
            }
            return View(model);
        }
        
        /// <summary>
        /// Displays a listing of all bikes in the system.
        /// </summary>
        /// <returns></returns>
        public ActionResult bikeListing(int page = 1)
        {
            var model = new bikeListingViewModel();
            model.allAvailableBikes = context.Bike.Where(l => !l.isArchived).Where(c => c.checkOuts.Where(i => !i.isResolved).Count() == 0)
                .Where(l => l.lastPassedInspection.AddDays(context.settings.First().DaysBetweenInspections) < DateTime.Now);
            
            model.countAvailableBikes = model.allAvailableBikes.Count();
            model.rentedBikes = 0;
            model.pagingInfo = new PageInfo(model.countAvailableBikes, pageSize, page);
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
            var availableBikes = context.Bike.Where(l => !l.isArchived).Where(c => c.checkOuts.Where(i => !i.isResolved).Count() == 0)
                .Where(l => l.lastPassedInspection < dateFloor);
            foreach(BikeRack rack in model.modelList)
            {
                rack.bikes = availableBikes.Where(r => r.bikeRack.bikeRackId == rack.bikeRackId).ToList();
            }
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
        public ActionResult userEdit(int userId, [Bind(Include="phoneNumber,email,firstName,lastName")] bikeUser user)
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
            rack.bikes = rack.bikes;
            return View(rack);
        }

        private string getUrlForRackImage(int rackId)
        {
            if( System.IO.File.Exists(Request.PhysicalApplicationPath.ToString() + "\\Content\\Images\\Racks\\" + rackId + ".jpg"))
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
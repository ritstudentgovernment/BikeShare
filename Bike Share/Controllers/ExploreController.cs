using BikeShare.Interfaces;
using BikeShare.Models;
using System.Linq;
using System.Text;
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
        private IExploreRepository repo;
        private IUserRepository userRepo;
        
        private int pageSize = 25;

        /// <summary>
        /// Initializes the controller with dependency injection.
        /// </summary>
        /// <param name="param">IExploreRepository implementation to use.</param>
        public ExploreController(IExploreRepository param, IUserRepository uParam)
        {
            repo = param;
            userRepo = uParam;
        }

        [Authorize]
        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Register(string firstName, string lastName)
        {
            userRepo.registerUser(User.Identity.Name, firstName, lastName);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Returns the Explore/Index page with no ViewModel.
        /// </summary>
        /// <returns>View without ViewModel.</returns>
        public ActionResult index(int page = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!userRepo.isUserRegistrationValid(User.Identity.Name))
                {
                    return RedirectToAction("Register");
                }
                var model = new BikeShare.ViewModels.profileViewModel();
                model.user = userRepo.getUserByName(User.Identity.Name);
                model.cards = new List<ActivityCard>();
                model.pagingInfo = new PageInfo(repo.countEventsForUser(model.user.bikeUserId), pageSize, (page - 1) * pageSize);
                foreach(var item in repo.getSomeEvents(model.user.bikeUserId, (page - 1) * pageSize, pageSize))
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
            else
            {
                return RedirectToAction("logIn", "Account");
            }
        }
        
        /// <summary>
        /// Displays a listing of all bikes in the system.
        /// </summary>
        /// <returns></returns>
        public ActionResult bikeListing(int page = 1)
        {
            var model = new bikeListingViewModel();
            model.allAvailableBikes = repo.getAvailableBikes();
            model.countAvailableBikes = repo.countAvailableBikes();
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
            model.modelList = repo.getAvailableRacks().ToList();
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
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        public ActionResult userEdit(int userId)
        {
            return View(userRepo.getUserById(userId));
        }

        [HttpPost]
        public ActionResult userEdit(int userId, [Bind(Include="phoneNumber,email,firstName,lastName")] bikeUser user)
        {
            var old = userRepo.getUserById(userId);
            userRepo.updateUser(userId, old.userName, user.email, user.phoneNumber, user.firstName, user.lastName);
            return RedirectToAction("Index", "Explore");
        }

        public ActionResult rackDetails(int rackId)
        {
            return View(repo.getAvailableRacks().Where(i => i.bikeRackId == rackId).First());
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
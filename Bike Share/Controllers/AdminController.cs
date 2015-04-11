using BikeShare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles site administration.
    /// </summary>
    [Authorize]
    public class AdminController : Controller
    {
        private int pageSize = 25;
        private BikesContext context;

        public AdminController()
        {
            context = new BikesContext();
        }

        private bool authorize()
        {
            try
            {
                return context.BikeUser.Where(n => n.userName == User.Identity.Name).First().canAdministerSite;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Displays the Application Administration home page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.dashViewModel();
            model.countBikes = context.Bike.Where(b => !b.isArchived).Count();
            model.countAvailableBikes = context.Bike.Where(b => !b.isArchived).Where(b => !b.checkOuts.Select(z => z.isResolved).Contains(false)).Count();
            model.countCharges = context.Charge.Count();
            model.countCheckouts = context.CheckOut.Count();
            model.countMaintenance = context.MaintenanceEvent.Count();
            model.countOngoingMaintenance = context.MaintenanceEvent.Where(m => !m.resolved).Count();
            model.countRacks = context.BikeRack.Where(r => !r.isArchived).Count();
            model.countInspections = context.Inspection.Count();
            model.countRacks = context.BikeUser.Where(a => !a.isArchived).Count();
            return View(model);
        }

        /// <summary>
        /// Displays the new bike form
        /// </summary>
        /// <returns></returns>
        public ActionResult newBike(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            ViewBag.query = context.BikeRack.Where(a => !a.isArchived).ToList();
            return View();
        }

        /// <summary>
        /// Submits a new bike to the system.
        /// </summary>
        /// <param name="bike">Bike to add.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newBike([Bind()] ViewModels.newBikeViewModel bikeModel)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.Bike.Add(new Bike { isArchived = false, bikeNumber = bikeModel.bikeNumber, 
                bikeName = bikeModel.bikeName, bikeRack = context.BikeRack.Find(bikeModel.bikeRackId), 
                lastCheckedOut = new DateTime(2000, 01, 01), lastPassedInspection = new DateTime(2000, 01, 01) }); //Date fields need to be explicitly set to prevent conversion error
            context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        /// <summary>
        /// Displays the form for creating a new bike rack.
        /// </summary>
        /// <returns></returns>
        public ActionResult newRack(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View();
        }

        /// <summary>
        /// Submits a new bike rack to the system.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newRack(BikeRack rack)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            rack.isArchived = false;
            context.BikeRack.Add(rack);
            context.SaveChanges();
            return RedirectToAction("bikeRackList");
        }

        /// <summary>
        /// Displays the warning page before archiving a bike.
        /// </summary>
        /// <returns></returns>
        public ActionResult archiveBike(int bikeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.Bike.Find(bikeId));
        }

        /// <summary>
        /// Submits the bike and archives it.
        /// </summary>
        /// <param name="bike"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult archiveBike([Bind(Include = "bikeId")]Bike bike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.Bike.Find(bike.bikeId).isArchived = true;
            context.SaveChanges();
            Response.RedirectToRoute(new { action = "Index", controller = "Admin" });
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the warning page before archiving a rack.
        /// </summary>
        /// <returns></returns>
        public ActionResult archiveRack(int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.BikeRack.Find(rackId));
        }

        /// <summary>
        /// Archives the provided rack.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult archiveRack([Bind(Include = "bikeRackId")] BikeRack rack)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.BikeRack.Find(rack.bikeRackId).isArchived = true;
            return RedirectToAction("Index");
        }

        public ActionResult archiveUser(int userId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.BikeUser.Find(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult archiveUser([Bind(Include = "bikeUserId")] bikeUser user)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.BikeUser.Find(user.bikeUserId).isArchived = true;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult adminList(int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = context.BikeUser.Where(a => a.canAdministerSite).OrderByDescending(d => d.userName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(context.BikeUser.Where(a => a.canAdministerSite).Count(), pageSize, page);
            return View(model);
        }

        public ActionResult appSettings(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.settings.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult appSettings([Bind] appSetting settings)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.settings.Remove(context.settings.First());
            context.settings.Add(settings);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult bikeList(int? rackId, int page = 1, bool incMissing = true, bool incOverdue = true, bool incCheckedOut = true, bool incCheckedIn = true, bool incCurrent = true, bool incArchived = false)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.FilteredBikeViewModel();
            var all = new List<Bike>();
            if (incCurrent)
            {
                all.AddRange(context.Bike.Include(b => b.bikeRack).Include(b => b.checkOuts).Where(a => !a.isArchived).ToList());
            }
            if (incArchived)
            {
                all.AddRange(context.Bike.Include(b => b.bikeRack).Include(b => b.checkOuts).Where(a => a.isArchived).ToList());
            }
            if (!incCheckedIn)
            {
                all.RemoveAll(b => b.checkOuts.All(c => c.isResolved));
            }
            if (!incCheckedOut)
            {
                all.RemoveAll(b => b.checkOuts.Any(c => !c.isResolved));
            }
            if (rackId != null)
            {
                all = all.Where(b => b.bikeRack.bikeRackId == rackId).ToList();
            }
            model.modelList = all;
            model.pagingInfo = new ViewModels.PageInfo(all.Count(), pageSize, page);
            model.modelList = all.OrderByDescending(d => d.bikeId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.includeArchived = incArchived;
            model.includeCheckedIn = incCheckedIn;
            model.includeCheckedOut = incCheckedOut;
            model.includeCurrent = incCurrent;
            model.includeMissing = incMissing;
            model.includeOverdue = incOverdue;
            return View(model);
        }

        public ActionResult bikeRackList(int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.rackListingViewModel();
            model.bikeRacks = context.BikeRack.Include(c => c.checkOuts).OrderByDescending(d => d.bikeRackId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.countBikeRacks = model.bikeRacks.Count();
            model.pagingInfo = new ViewModels.PageInfo(context.BikeRack.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult inspectionList(int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            var model = new ViewModels.inspectionListViewModel();
            model.inspections = context.Inspection.Include(c => c.bike).Include(c => c.inspector).OrderByDescending(d => d.datePerformed).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.inspections.Count(), pageSize, page);
            return View(model);
        }

        /// <summary>
        /// TODO - filter for bike
        /// </summary>
        /// <param name="bikeId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult maintenanceList(int? bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            var model = new ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = context.MaintenanceEvent.Include(c => c.updates).Include(c => c.staffPerson).Include(c => c.bikeAffected).OrderByDescending(d => d.timeAdded).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult mechanicList(int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = context.BikeUser.OrderByDescending(d => d.userName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult userList(string name = "", int page = 1, bool hasCharges = false, bool hasBike = false, bool canMaintain = false, bool canAdmin = false, bool canRide = false, bool canCheckout = false)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = context.BikeUser.Where(c => c.canBorrowBikes == canRide).Where(c => c.canAdministerSite == canAdmin).Where(c => c.canCheckOutBikes == canCheckout).Where(c => c.canMaintainBikes == canMaintain).ToList();
            if (hasCharges)
            {
                model.modelList = model.modelList.Where(c => context.Charge.Where(i => i.chargeId == c.bikeUserId).Count() < 1).ToList();
            }
            int totalResults = model.modelList.Count();
            model.modelList = model.modelList.OrderByDescending(d => d.userName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalMechanics = context.BikeUser.Where(a => a.canMaintainBikes).Count();
            int totalCheckout = context.BikeUser.Where(c => c.canCheckOutBikes).Count();
            int totalAdmin = context.BikeUser.Where(c => c.canAdministerSite).Count();
            int totalRiders = context.BikeUser.Where(c => c.canBorrowBikes).Count();
            ViewBag.canRide = canRide; ViewBag.canMaintain = canMaintain; ViewBag.canAdmin = canAdmin; ViewBag.canCheckout = canCheckout;
            model.pagingInfo = new ViewModels.PageInfo(totalResults, pageSize, page);
            return View(model);
        }

        public ActionResult workshopList(int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<Workshop>();

            model.modelList = context.WorkShop.Include(m => m.maintenanceEvents).OrderByDescending(d => d.workshopId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(context.WorkShop.Count(), pageSize, page);
            return View(model);
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="bikeId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult bikeCheckouts(int? rackId, int? bikeID, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<CheckOut>();

            IQueryable<CheckOut> list;
            if (bikeID == null && rackId == null)
            {
                list = context.CheckOut.Include(b => b.bike).Include(u => u.user);
            }
            else if (rackId != null)
            {
                list = context.CheckOut.Include(b => b.bike).Include(r => r.rackCheckedIn).Include(r => r.rackCheckedOut).Where(u => u.rackCheckedOut.bikeRackId == rackId || u.rackCheckedIn.bikeRackId == rackId).Include(u => u.user);
            }
            else
            {
                list = context.CheckOut.Include(b => b.bike).Where(b => b.bike.bikeId == bikeID).Include(r => r.rackCheckedIn).Include(r => r.rackCheckedOut).Include(u => u.user);
            }
            int total = list.Count();
            model.modelList = list.OrderByDescending(d => d.timeOut).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(total, pageSize, page);
            return View(model);
        }

        public ActionResult editBike(int bikeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            return View(context.Bike.Find(bikeId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editBike([Bind] Bike bike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            var dbike = context.Bike.Find(bike.bikeId);
            dbike.bikeName = bike.bikeName;
            dbike.bikeNumber = bike.bikeNumber;
            dbike.isArchived = bike.isArchived;
            context.SaveChanges();

            return RedirectToAction("bikeList");
        }

        public ActionResult infoBike(int bikeID)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.superBike();

            model.bike = context.Bike.Find(bikeID);
            model.inspections = context.Inspection.Where(b => b.bike.bikeId == bikeID).OrderByDescending(d => d.datePerformed).Take(25).ToList();
            model.maintenance = context.MaintenanceEvent.Where(b => b.bikeAffected.bikeId == bikeID).OrderByDescending(d => d.timeAdded).Take(25).ToList();
            return View(model);
        }

        public ActionResult editRack(int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            return View(context.BikeRack.Find(rackId));
        }

        /// <summary>
        /// Submits a new bike rack to the system.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editRack([Bind] BikeRack rack)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            var dRack = context.BikeRack.Find(rack.bikeRackId);
            dRack.description = rack.description;
            dRack.GPSCoordX = rack.GPSCoordX;
            dRack.GPSCoordY = rack.GPSCoordY;
            dRack.isArchived = rack.isArchived;
            dRack.name = rack.name;
            context.SaveChanges();

            return RedirectToAction("editRack", new { rackId = rack.bikeRackId });
        }

        public ActionResult newUser(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(new BikeShare.ViewModels.bikeUserPermissionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newUser([Bind] BikeShare.ViewModels.bikeUserPermissionViewModel user)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            context.BikeUser.Add(new bikeUser
            {
                userName = user.userName,
                phoneNumber = user.phone,
                isArchived = false,
                hasBike = false,
                email = user.email,
                canMaintainBikes = user.canMaintainBikes,
                canCheckOutBikes = user.canCheckOutBikes,
                canAdministerSite = user.canManageApp,
                canBorrowBikes = user.canBorrowBikes
            });
            context.SaveChanges();

            return RedirectToAction("userList", "Admin");
        }

        public ActionResult newWorkshop(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(new Workshop());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newWorkshop([Bind] Workshop shop)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.WorkShop.Add(shop);
            context.SaveChanges();
            return RedirectToAction("workshopList");
        }

        public ActionResult archiveWorkshop(int workshopId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.WorkShop.Find(workshopId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult archiveWorkshop([Bind] Workshop shop)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.WorkShop.Find(shop.workshopId).isArchived = true;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult bikeMaintenance(int bikeId = 1, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<MaintenanceEvent>();
            IQueryable<MaintenanceEvent> list = context.MaintenanceEvent.Where(b => b.bikeAffected.bikeId == bikeId);
            int total = list.Count();
            model.modelList = list.OrderByDescending(d => d.timeAdded).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(total, pageSize, page);
            return View(model);
        }

        public ActionResult userDetails(int userId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.BikeUser.Find(userId));
        }

        public ActionResult userEdit(int userId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.BikeUser.Find(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult userEdit([Bind] bikeUser user)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var dUser = context.BikeUser.Find(user.bikeUserId);
            dUser.canAdministerSite = user.canAdministerSite;
            dUser.canBorrowBikes = user.canBorrowBikes;
            dUser.canCheckOutBikes = user.canCheckOutBikes;
            dUser.canMaintainBikes = user.canMaintainBikes;
            dUser.email = user.email;
            dUser.firstName = user.firstName;
            dUser.lastName = user.lastName;
            dUser.isArchived = user.isArchived;
            dUser.phoneNumber = user.phoneNumber;
            return RedirectToAction("userDetails", new { userId = user.bikeUserId });
        }

        public ActionResult workshopDetails(int workshopId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.WorkShop.Find(workshopId));
        }

        public ActionResult chargesList(int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<Charge>();
            model.modelList = context.Charge.OrderByDescending(d => d.chargeId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(context.Charge.Count(), pageSize, page);
            ViewBag.totalCharges = context.Charge.Count();
            ViewBag.totalResolved = context.Charge.Where(i => i.isResolved).Count();
            ViewBag.totalPaid = context.Charge.Sum(c => c.amountPaid);
            ViewBag.totalUnpaid = context.Charge.Sum(c => c.amountCharged) - ViewBag.totalPaid;
            return View(model);
        }

        public ActionResult chargeDetails(int chargeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.Charge.Find(chargeId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult closeCharge(int chargeId, decimal amountPaid)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var x = context.Charge.Find(chargeId);
            x.amountPaid = amountPaid;
            x.isResolved = true;
            x.dateResolved = DateTime.Now;
            context.SaveChanges();
            return View("chargeDetails", x);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editCharge(int chargeId, decimal amountCharged, string chargeTitle, string chargeDescription)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var x = context.Charge.Find(chargeId);
            x.amountCharged = amountCharged;
            x.title = chargeTitle;
            x.description = chargeDescription;
            context.SaveChanges();
            return RedirectToAction("chargeDetails", "Admin", new { chargeId = chargeId });
        }

        public ActionResult newCharge(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(new Charge());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newCharge(decimal amountCharged, string chargeTitle, string chargeDescription, string chargeUser)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.Charge.Add(new Charge { amountCharged = amountCharged, title = chargeTitle, dateAssesed = DateTime.Now, description = chargeDescription, user = context.BikeUser.Where(u => u.userName == chargeUser).First() });
            context.SaveChanges();
            return RedirectToAction("chargesList", "Admin");
        }

        public ActionResult newHour(int? workshopId, int? rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            if (workshopId != null)
            {
                var model = new BikeShare.Models.Hour();
                model.shop = context.WorkShop.Find(workshopId);
                return View(model);
            }
            if (rackId != null)
            {
                var model = new BikeShare.Models.Hour();
                model.rack = context.BikeRack.Find(rackId);
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newHour(int? workshopId, int? rackId, [Bind] Hour hour)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            if (workshopId != null)
            {
                hour.shop = context.WorkShop.Find(workshopId);
                hour.shop.hours.Add(hour);
                return RedirectToAction("workshopDetails", "Admin", new { shopId = (int)workshopId });
            }
            if (rackId != null)
            {
                hour.rack = context.BikeRack.Find(rackId);
                hour.rack.hours.Add(hour);
                return RedirectToAction("editRack", "Admin", new { rackId = (int)rackId });
            }
            return RedirectToAction("Index");
        }

        public ActionResult newComment(int maintId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new MaintenanceUpdate();
            model.associatedEvent = context.MaintenanceEvent.Find(maintId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newComment(int maintenanceId, string commentTitle, string commentBody)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            context.MaintenanceEvent.Find(maintenanceId).updates.Add(new MaintenanceUpdate
            {
                body = commentBody,
                timePosted = DateTime.Now,
                title = commentTitle,
                postedBy = context.BikeUser.Where(u => u.userName == User.Identity.Name).First()
            });
            return RedirectToAction("MaintenanceDetails", new { maintId = maintenanceId });
        }

        public ActionResult maintenanceDetails(int maintId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.MaintenanceEvent.Find(maintId));
        }

        public ActionResult uploadImage(int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.BikeRack.Find(rackId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult uploadImage(int rackId, string image)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            HttpPostedFileBase file = Request.Files["image"];
            byte[] tempImage = new byte[file.ContentLength];
            file.InputStream.Read(tempImage, 0, file.ContentLength);
            file.SaveAs(Request.PhysicalApplicationPath.ToString() + "\\Content\\Images\\Racks\\" + rackId + ".jpg");
            return RedirectToAction("editRack", new { rackId = rackId });
        }

        public ActionResult doesUserExist(string validationName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var x = Json(context.BikeUser.Where(u => u.userName == validationName).Count() > 0, JsonRequestBehavior.AllowGet);
            return x;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult hourDelete(int? workshopId, int? rackId, int hourId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            if (workshopId != null)
            {
                var shop = context.WorkShop.Find(workshopId);
                context.workHours.Remove(context.workHours.Find(hourId));
                return RedirectToAction("workshopDetails", "Admin", new { shopId = (int)workshopId });
            }
            if (rackId != null)
            {
                var rack = context.BikeRack.Find(rackId);
                context.hour.Remove(context.hour.Find(hourId));
                return RedirectToAction("editRack", "Admin", new { rackId = (int)rackId });
            }
            return RedirectToAction("Index");
        }

        public ActionResult reports()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View();
        }

        [HttpPost]
        public ActionResult checkoutReport(DateTime start, DateTime end)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            DisplayLogFile(generateCheckoutLog(start, end));

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult bikeReport()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            DisplayLogFile(generateBikeLog());

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult rackReport()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            DisplayLogFile(generateRackLog());

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult userReport()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            DisplayLogFile(generateUserLog());

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult inspectionReport(DateTime start, DateTime end)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            DisplayLogFile(generateInspectionLog(start, end));

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult maintReport(DateTime start, DateTime end)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            DisplayLogFile(generateMaintenanceLog(start, end));

            return RedirectToAction("reports");
        }

        // <summary>
        /// Transmits the csv file to the browser.
        /// </summary>
        /// <param name="csvExportContents">String contents of the csv file.</param>
        private void DisplayLogFile(string csvExportContents)
        {
            byte[] data = new ASCIIEncoding().GetBytes(csvExportContents);

            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "APPLICATION/OCTET-STREAM";
            HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=Export.csv");
            HttpContext.Response.OutputStream.Write(data, 0, data.Length);
            HttpContext.Response.End();
        }

        private string generateCheckoutLog(DateTime start, DateTime end)
        {
            StringBuilder csvExport = new StringBuilder();
            csvExport.AppendLine(String.Format("Checkout Report - {0} to {1}", start, end));

            csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    "Rider", "Time Out", "Time In", "Rack Out", "Rack In", "Rental Complete?", "Bike Number"));
            foreach (var checkout in context.CheckOut)
            {
                csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    checkout.user.userName, checkout.timeOut, checkout.timeIn, checkout.rackCheckedOut.name, checkout.rackCheckedIn.name, checkout.isResolved, checkout.bike.bikeNumber));
            }

            return csvExport.ToString();
        }

        private string generateBikeLog()
        {
            StringBuilder csvExport = new StringBuilder();
            csvExport.AppendLine("Bike Report");

            csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                    "Bike Number", "Bike Name", "Archived?", "Last Checked Out", "Last Passed Inspection", "Total Inspections"));
            foreach (var bike in context.Bike)
            {
                csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                    bike.bikeNumber, bike.bikeName, bike.isArchived, bike.lastCheckedOut.ToString(), bike.lastPassedInspection.ToString(), context.Inspection.Where(b => b.bike == bike).Count()));
            }

            return csvExport.ToString();
        }

        private string generateRackLog()
        {
            StringBuilder csvExport = new StringBuilder();
            csvExport.AppendLine("Bike Report");

            csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                    "Rack Name", "Archived?", "Latitude", "Longitude", "Description"));
            foreach (var Rack in context.BikeRack)
            {
                csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                    Rack.name, Rack.isArchived, Rack.GPSCoordX, Rack.GPSCoordY, Rack.description));
            }

            return csvExport.ToString();
        }

        private string generateUserLog()
        {
            StringBuilder csvExport = new StringBuilder();
            csvExport.AppendLine("User Report");

            csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",
                    "User Name", "First Name", "Last Name", "Phone", "Email", "Last Registered", "Checkout Privileges?", "Rider Privileges?", "Mechanic Privileges?", "Admin Privileges?"));
            foreach (var User in context.BikeUser)
            {
                csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",
                    User.userName, User.firstName, User.lastName, User.phoneNumber, User.email, User.lastRegistered, User.canCheckOutBikes, User.canBorrowBikes, User.canMaintainBikes, User.canAdministerSite));
            }

            return csvExport.ToString();
        }

        private string generateInspectionLog(DateTime start, DateTime end)
        {
            StringBuilder csvExport = new StringBuilder();
            csvExport.AppendLine("User Report");

            csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                    "Bike Name", "Bike Number", "Date Performed", "Comment", "Passed?"));
            foreach (var Inspection in context.Inspection)
            {
                csvExport.AppendLine(
                string.Format(
                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                Inspection.bike.bikeName, Inspection.bike.bikeNumber, Inspection.datePerformed, Inspection.comment, Inspection.isPassed));
            }

            return csvExport.ToString();
        }

        private string generateMaintenanceLog(DateTime start, DateTime end)
        {
            StringBuilder csvExport = new StringBuilder();
            csvExport.AppendLine("User Report");

            csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                    "Bike Name", "Bike Number", "Title", "Time Added", "Time Resolved", "Resolved?", "Archived?", "Bike Disabled?", "Details"));
            foreach (var Maint in context.MaintenanceEvent)
            {
                csvExport.AppendLine(
                string.Format(
                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                Maint.bikeAffected.bikeName, Maint.bikeAffected.bikeNumber, Maint.title, Maint.timeAdded, Maint.timeResolved, Maint.resolved, Maint.isArchived, Maint.disableBike, Maint.details));
            }

            return csvExport.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
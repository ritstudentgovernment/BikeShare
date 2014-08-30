using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Models;
using BikeShare.Interfaces;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles site administration.
    /// </summary>
   [Authorize]
    public class AdminController : Controller
    {
        private IAdminRepository repo;
        private IWorkshopRepository workshopRepo;
        private IUserRepository userRepo;
        private IFinanceRepository financeRepo;
        private IMaintenanceRepository maintRepo;
        private ISettingRepository settingRepo;
        private int pageSize = 25;

        public AdminController(IAdminRepository param, IWorkshopRepository wParam, IUserRepository uParam, IFinanceRepository fParam, IMaintenanceRepository mParam, ISettingRepository sParam)
        {
            repo = param;
            workshopRepo = wParam;
            userRepo = uParam;
            financeRepo = fParam;
            maintRepo = mParam;
            settingRepo = sParam;
        }

       private bool authorize()
        {
           if (!userRepo.canUserManageApp(User.Identity.Name))
           {
               return false;
           }
           return true;
        }
        /// <summary>
        /// Displays the Application Administration home page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.dashViewModel();
            model.countAvailableBikes = repo.totalAvailableBikes();
            model.countBikes = repo.totalBikes();
            model.countCharges = financeRepo.countTotalCharges();
            model.countCheckouts = repo.totalCheckOuts();
            model.countInspections = repo.totalInspections();
            model.countMaintenance = repo.totalMaintenances();
            model.countOngoingMaintenance = repo.getAllMaintenance().Where(r => !r.resolved).Count(); //TODO - Don't do this
            model.countRacks = repo.totalRacks();
            model.countUsers = userRepo.totalUsers(false, true);
            return View(model);
        }

        /// <summary>
        /// Displays the new bike form
        /// </summary>
        /// <returns></returns>
       public ActionResult newBike(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            ViewBag.query = repo.getAllBikeRacks().ToList();
            return View();
        }

        /// <summary>
        /// Submits a new bike to the system.
        /// </summary>
        /// <param name="bike">Bike to add.</param>
        /// <returns></returns>
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult newBike( [Bind()] ViewModels.newBikeViewModel bikeModel)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var bike = new Bike();
            bike.bikeName = bikeModel.bikeName;
            bike.bikeNumber = bikeModel.bikeNumber;
            bike.lastCheckedOut = new DateTime(2000, 1, 1);
            bike.bikeRack = repo.getAllBikeRacks().Where(b => b.bikeRackId == bikeModel.bikeRackId).First();
            bike.isArchived = false;
            repo.addBike(bike);
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
       public ActionResult newRack( BikeRack rack)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            rack.isArchived = false;
            repo.addBikeRack(rack);
            return RedirectToAction("bikeRackList");
        }

        /// <summary>
        /// Displays the warning page before archiving a bike.
        /// </summary>
        /// <returns></returns>
       public ActionResult archiveBike( int bikeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getBikeById(bikeId));
        }

        /// <summary>
        /// Submits the bike and archives it.
        /// </summary>
        /// <param name="bike"></param>
        /// <returns></returns>
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult archiveBike( [Bind(Include="bikeId")]Bike bike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            repo.archiveBike(bike.bikeId);
            Response.RedirectToRoute(new { action = "Index", controller = "Admin"});
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the warning page before archiving a rack.
        /// </summary>
        /// <returns></returns>
       public ActionResult archiveRack( int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getRackById(rackId));
        }

        /// <summary>
        /// Archives the provided rack.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult archiveRack( [Bind(Include="bikeRackId")] BikeRack rack)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            repo.archiveBikeRack(rack.bikeRackId);
            return RedirectToAction("Index");
        }

        public ActionResult archiveUser( int userId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(userRepo.getUserById(userId));
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult archiveUser( [Bind(Include="bikeUserId")] bikeUser user)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            userRepo.archiveUser(user.bikeUserId);
            return RedirectToAction("Index");
        }

        public ActionResult adminList( int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = userRepo.getSomeUsers(pageSize, (page - 1) * pageSize, false, false, false, true, false, true).ToList();
            model.pagingInfo = new ViewModels.PageInfo(repo.totalAppAdmins(), pageSize, page);
            return View(model);
        }

        public ActionResult appSettings(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new appSetting();
            model.appName = settingRepo.getappName();
            model.DaysBetweenInspections = settingRepo.getDaysBetweenInspections();
            model.expectedEmail = settingRepo.getexpectedEmail();
            model.maxRentDays = settingRepo.getmaxRentDays();
            return View(model);
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult appSettings( [Bind] appSetting settings)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            settingRepo.setMaxRentDays(settings.maxRentDays);
            settingRepo.setExpectedEmail(settings.expectedEmail);
            settingRepo.setDaysBetweenInspections(settings.DaysBetweenInspections);
            settingRepo.setAppName(settings.appName);
            return RedirectToAction("Index");
        }

        public ActionResult bikeList( int? rackId, int page = 1, bool incMissing = true, bool incOverdue = true, bool incCheckedOut = true, bool incCheckedIn = true, bool incCurrent = true, bool incArchived = false)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.FilteredBikeViewModel();
            var all = new List<Bike>();
            if (incCurrent)
            {
                all.AddRange(repo.getAllBikes());
            }
            if (incArchived)
            {
                all.AddRange(repo.getArchivedBikes(1000, 0));
            }
            if(!incCheckedIn)
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
            model.modelList = all.Skip((page -1) * pageSize).Take(pageSize).ToList();

            model.includeArchived = incArchived;
            model.includeCheckedIn = incCheckedIn;
            model.includeCheckedOut = incCheckedOut;
            model.includeCurrent = incCurrent;
            model.includeMissing = incMissing;
            model.includeOverdue = incOverdue;
            return View(model);
        }

        public ActionResult bikeRackList( int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.rackListingViewModel();
            model.bikeRacks = repo.getSomeBikeRacks(pageSize, (page - 1) * pageSize);
            model.countBikeRacks = model.bikeRacks.Count();
            model.pagingInfo = new ViewModels.PageInfo(repo.totalRacks(), pageSize, page);
            return View(model);
        }

        public ActionResult inspectionList( int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.inspectionListViewModel();
            model.inspections = repo.getSomeInspections(pageSize, (page - 1) * pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.inspections.Count(), pageSize, page);
            return View(model);
        }

        /// <summary>
        /// TODO - filter for bike
        /// </summary>
        /// <param name="bikeId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
       public ActionResult maintenanceList( int? bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = repo.getSomeMaintenance(pageSize, (page - 1) * pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult mechanicList( int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = repo.getSomeUsers(pageSize, (page - 1) * pageSize, false, false, true, false).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult userList( string name = "", int page = 1, bool hasCharges = false, bool hasBike = false, bool canMaintain = false, bool canAdmin = false, bool canRide = false, bool canCheckout = false)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = repo.getFilteredUsers(pageSize, (page - 1) * pageSize, hasCharges, hasBike, canMaintain, canAdmin, canRide, canCheckout, name.ToString()).ToList();
            int totalResults = userRepo.totalUsers();
            int totalMechanics = userRepo.totalMechanics();
            int totalCheckout = userRepo.totalCheckOutPeople();
            int totalAdmin = userRepo.totalAppAdmins();
            int totalRiders = userRepo.totalRiders();
            ViewBag.canRide = canRide; ViewBag.canMaintain = canMaintain; ViewBag.canAdmin = canAdmin; ViewBag.canCheckout = canCheckout;
            if (canMaintain) { if (totalMechanics < totalResults) { totalResults = totalMechanics; } }
            if (canCheckout) { if (totalCheckout < totalResults) { totalResults = totalCheckout; } }
            if (canAdmin) { if (totalAdmin < totalResults) { totalResults = totalAdmin; } }
            if (canRide) { if (totalRiders < totalResults) { totalResults = totalRiders; } }
            model.pagingInfo = new ViewModels.PageInfo(totalResults, pageSize, page);
            return View(model);
        }

        public ActionResult workshopList( int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<Workshop>();
            model.modelList = repo.getSomeWorkshops(pageSize, (page - 1) * pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="bikeId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
       public ActionResult bikeCheckouts( int? rackId, int? bikeID, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<CheckOut>();
            if(bikeID == null && rackId == null)
            {
                model.modelList = repo.getSomeCheckouts(pageSize, (page - 1) * pageSize, false).ToList();
            }
            else if (rackId != null)
            {
                model.modelList = repo.getAllCheckouts().Where(r => { if (r.rackCheckedIn == null) { return r.rackCheckedOut.bikeRackId == rackId; } else { return r.rackCheckedIn.bikeRackId == rackId; } }).ToList();
            }
            else
            {
                model.modelList = repo.getBikesCheckouts((int)bikeID, pageSize, (page - 1) * pageSize).ToList();
            }

            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult editBike( int bikeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getBikeById(bikeId));
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult editBike( [Bind] Bike bike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            repo.updateBike(bike);
            return RedirectToAction("bikeList");
        }

        public ActionResult infoBike( int bikeID)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.superBike();
            model.bike = repo.getBikeById(bikeID);
            model.inspections = maintRepo.getInspectionsForBike(bikeID, 0, pageSize, true, true).ToList();
            model.maintenance = maintRepo.getMaintenanceForBike(bikeID, 0, pageSize, false).ToList();
            return View(model);
        }

        public ActionResult editRack( int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getRackById(rackId));
        }

        /// <summary>
        /// Submits a new bike rack to the system.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult editRack( [Bind] BikeRack rack)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            repo.updateBikeRack(rack);
            return RedirectToAction("editRack", new { rackId = rack.bikeRackId});
        }

        public ActionResult newUser(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(new BikeShare.ViewModels.bikeUserPermissionViewModel());
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult newUser( [Bind] BikeShare.ViewModels.bikeUserPermissionViewModel user)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            userRepo.createuser(user.userName, user.email, user.phone, user.canCheckOutBikes, user.canBorrowBikes, user.canMaintainBikes, user.canManageApp);
            return RedirectToAction("userList", "Admin");
        }

        public ActionResult newWorkshop(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(new Workshop());
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult newWorkshop( [Bind] Workshop shop)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            workshopRepo.createNewWorkshop(shop.name, shop.GPSCoordX, shop.GPSCoordY);
            return RedirectToAction("workshopList");
        }

        public ActionResult archiveWorkshop( int workshopId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(workshopRepo.getWorkshopById(workshopId));
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult archiveWorkshop( [Bind] Workshop shop)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            workshopRepo.archiveWorkshopById(shop.workshopId);
            return RedirectToAction("Index");
        }

        public ActionResult bikeMaintenance( int bikeId = 1, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = repo.getMaintenanceForBike(pageSize, (page - 1) * pageSize, bikeId).ToList();
            model.pagingInfo = new ViewModels.PageInfo(500, pageSize, page);
            return View(model);
        }

        public ActionResult userDetails( int userId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getUserById(userId));
        }

        public ActionResult userEdit( int userId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getUserById(userId));
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult userEdit( [Bind] bikeUser user)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            repo.updateUser(user);
            return RedirectToAction("userDetails", new { userId = user.bikeUserId });
        }

        public ActionResult workshopDetails( int workshopId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getWorkshopById(workshopId));
        }

        public ActionResult chargesList( int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<Charge>();
            model.modelList = financeRepo.getAllCharges(pageSize, (page - 1) * pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(financeRepo.countTotalCharges(), pageSize, page);
            ViewBag.totalCharges = financeRepo.countTotalCharges();
            ViewBag.totalResolved = financeRepo.countResolvedCharges();
            ViewBag.totalPaid = financeRepo.incomeToDate();
            ViewBag.totalUnpaid = financeRepo.outstandingBalance();
            return View(model);
        }

        public ActionResult chargeDetails( int chargeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getChargeById(chargeId));
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult closeCharge( int chargeId, decimal amountPaid)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            financeRepo.closeCharge(chargeId, amountPaid);
            return View("chargeDetails", repo.getChargeById(chargeId));
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult editCharge( int chargeId, decimal amountCharged, string chargeTitle, string chargeDescription)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            financeRepo.updateCharge(chargeId, amountCharged, chargeTitle, chargeDescription);
            return RedirectToAction("chargeDetails", "Admin", new { chargeId = chargeId });
        }

        public ActionResult newCharge(string userName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(new Charge());
        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult newCharge( decimal amountCharged, string chargeTitle, string chargeDescription, string chargeUser)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            financeRepo.addCharge(amountCharged, chargeUser, chargeTitle, chargeDescription);
            return RedirectToAction("chargesList", "Admin");
        }

        public ActionResult newHour( int? workshopId, int? rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            if (workshopId != null)
            {
                var model = new BikeShare.Models.Hour();
                model.shop = workshopRepo.getWorkshopById((int)workshopId);
                return View(model);
            }
            if (rackId != null)
            {
                var model = new BikeShare.Models.Hour();
                model.rack = repo.getRackById((int)rackId);
                return View(model);
            }
            return RedirectToAction("Index");
        }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult newHour( int? workshopId, int? rackId, [Bind] Hour hour)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
           if(workshopId != null)
           {
               hour.shop = workshopRepo.getWorkshopById((int)workshopId);
               workshopRepo.addHour(hour);
               return RedirectToAction("workshopDetails", "Admin", new { shopId = (int)workshopId });
           }
           if(rackId != null)
           {
               hour.rack = repo.getRackById((int)rackId);
               repo.addHourToRack(hour);
               return RedirectToAction("editRack", "Admin", new { rackId = (int)rackId });
           }
           return RedirectToAction("Index");
        }

       public ActionResult newComment( int maintId)
       {
           if (!authorize()) { return RedirectToAction("authError", "Error"); }
           var model = new MaintenanceUpdate();
           model.associatedEvent = repo.getMaintenanceById(maintId);
           return View(model);
       }
       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult newComment( int maintenanceId, string commentTitle, string commentBody)
       {
           if (!authorize()) { return RedirectToAction("authError", "Error"); }
           maintRepo.commentOnMaint(maintenanceId, commentTitle, commentBody, User.Identity.Name);
           return RedirectToAction("MaintenanceDetails", new { maintId = maintenanceId });
       }

       public ActionResult maintenanceDetails( int maintId)
       {
           if (!authorize()) { return RedirectToAction("authError", "Error"); }
           return View(maintRepo.getMaintenanceById(maintId));
       }

       public ActionResult uploadImage( int rackId)
       {
           if (!authorize()) { return RedirectToAction("authError", "Error"); }
           return View(repo.getRackById(rackId));
       }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult uploadImage( int rackId, string image)
       {
           if (!authorize()) { return RedirectToAction("authError", "Error"); }
           HttpPostedFileBase file = Request.Files["image"];
           byte[] tempImage = new byte[file.ContentLength];
           file.InputStream.Read(tempImage, 0, file.ContentLength);
           file.SaveAs(Request.PhysicalApplicationPath.ToString() + "\\Content\\Images\\Racks\\" + rackId + ".jpg");
           return RedirectToAction("editRack", new { rackId = rackId });
       }

       public ActionResult doesUserExist( string validationName)
       {
           if (!authorize()) { return RedirectToAction("authError", "Error"); }
           var x = Json(userRepo.doesUserExist(validationName), JsonRequestBehavior.AllowGet);
            return x;

       }
       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult hourDelete( int? workshopId, int? rackId, int hourId)
       {
           if (!authorize()) { return RedirectToAction("authError", "Error"); }
           if (workshopId != null)
           {
               var shop = workshopRepo.getWorkshopById((int)workshopId);
               workshopRepo.deleteHourById(hourId);
               return RedirectToAction("workshopDetails", "Admin", new { shopId = (int)workshopId });
           }
           if (rackId != null)
           {
               var rack = repo.getRackById((int)rackId);
               repo.deleteHourById(hourId);
               return RedirectToAction("editRack", "Admin", new { rackId = (int)rackId });
           }
           return RedirectToAction("Index");
       }
    }
}
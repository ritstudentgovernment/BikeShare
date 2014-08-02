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
        /// <summary>
        /// Displays the Application Administration home page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
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
        public ActionResult newBike()
        {
            ViewBag.query = repo.getAllBikeRacks().ToList();
            return View();
        }

        /// <summary>
        /// Submits a new bike to the system.
        /// </summary>
        /// <param name="bike">Bike to add.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult newBike([Bind()] ViewModels.newBikeViewModel bikeModel)
        {
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
        public ActionResult newRack()
        {
            return View();
        }

        /// <summary>
        /// Submits a new bike rack to the system.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult newRack(BikeRack rack)
        {
            rack.isArchived = false;
            repo.addBikeRack(rack);
            return RedirectToAction("bikeRackList");
        }

        /// <summary>
        /// Displays the warning page before archiving a bike.
        /// </summary>
        /// <returns></returns>
        public ActionResult archiveBike(int bikeId)
        {
            return View(repo.getBikeById(bikeId));
        }

        /// <summary>
        /// Submits the bike and archives it.
        /// </summary>
        /// <param name="bike"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult archiveBike([Bind(Include="bikeId")]Bike bike)
        {
            repo.archiveBike(bike.bikeId);
            Response.RedirectToRoute(new { action = "Index", controller = "Admin"});
            return Index();
        }

        /// <summary>
        /// Displays the warning page before archiving a rack.
        /// </summary>
        /// <returns></returns>
        public ActionResult archiveRack(int rackId)
        {
            return View(repo.getRackById(rackId));
        }

        /// <summary>
        /// Archives the provided rack.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult archiveRack([Bind(Include="bikeRackId")] BikeRack rack)
        {
            repo.archiveBikeRack(rack.bikeRackId);
            return Index();
        }

        public ActionResult archiveUser(int userId)
        {
            
            return View(userRepo.getUserById(userId));
        }

        [HttpPost]
        public ActionResult archiveUser([Bind(Include="bikeUserId")] bikeUser user)
        {
            userRepo.archiveUser(user.bikeUserId);
            return Index();
        }

        public ActionResult adminList(int page = 1)
        {
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = userRepo.getSomeUsers(pageSize, (page - 1) * pageSize, false, false, false, true, false, true).ToList();
            model.pagingInfo = new ViewModels.PageInfo(repo.totalAppAdmins(), pageSize, page);
            return View(model);
        }

        public ActionResult appSettings()
        {
            var model = new appSetting();
            model.appName = settingRepo.getappName();
            model.DaysBetweenInspections = settingRepo.getDaysBetweenInspections();
            model.expectedEmail = settingRepo.getexpectedEmail();
            model.maxRentDays = settingRepo.getmaxRentDays();
            return View(model);
        }

        [HttpPost]
        public ActionResult appSettings([Bind] appSetting settings)
        {
            settingRepo.setMaxRentDays(settings.maxRentDays);
            settingRepo.setExpectedEmail(settings.expectedEmail);
            settingRepo.setDaysBetweenInspections(settings.DaysBetweenInspections);
            settingRepo.setAppName(settings.appName);
            return RedirectToAction("Index");
        }

        public ActionResult bikeList(int? rackId, int page = 1, bool incMissing = true, bool incOverdue = true, bool incCheckedOut = true, bool incCheckedIn = true, bool incCurrent = true, bool incArchived = false)
        {
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

        public ActionResult bikeRackList(int page = 1)
        {
            var model = new ViewModels.rackListingViewModel();
            model.bikeRacks = repo.getSomeBikeRacks(pageSize, (page - 1) * pageSize);
            model.countBikeRacks = model.bikeRacks.Count();
            model.pagingInfo = new ViewModels.PageInfo(repo.totalRacks(), pageSize, page);
            return View(model);
        }

        public ActionResult inspectionList(int page = 1)
        {
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
        public ActionResult maintenanceList(int? bikeId, int page = 1)
        {
            var model = new ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = repo.getSomeMaintenance(pageSize, (page - 1) * pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult mechanicList(int page = 1)
        {
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = repo.getSomeUsers(pageSize, (page - 1) * pageSize, false, false, true, false).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult userList(string name = "", int page = 1, bool hasCharges = false, bool hasBike = false, bool canMaintain = true, bool canAdmin = true, bool canRide = true, bool canCheckout = true)
        {
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = repo.getFilteredUsers(pageSize, (page - 1) * pageSize, hasCharges, hasBike, canMaintain, canAdmin, canRide, canCheckout, name.ToString()).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult workshopList(int page = 1)
        {
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
        public ActionResult bikeCheckouts(int? rackId, int? bikeID, int page = 1)
        {
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

        public ActionResult editBike(int bikeId)
        {
            return View(repo.getBikeById(bikeId));
        }

        [HttpPost]
        public ActionResult editBike([Bind] Bike bike)
        {
            repo.updateBike(bike);
            return RedirectToAction("bikeList");
        }

        public ActionResult infoBike(int bikeID)
        {
            var model = new BikeShare.ViewModels.superBike();
            model.bike = repo.getBikeById(bikeID);
            model.inspections = maintRepo.getInspectionsForBike(bikeID, 0, pageSize, true, true).ToList();
            model.maintenance = maintRepo.getMaintenanceForBike(bikeID, 0, pageSize, false).ToList();
            return View(model);
        }

        public ActionResult editRack(int rackId)
        {
            return View(repo.getRackById(rackId));
        }

        /// <summary>
        /// Submits a new bike rack to the system.
        /// </summary>
        /// <param name="rack"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult editRack([Bind] BikeRack rack)
        {
            repo.updateBikeRack(rack);
            return RedirectToAction("editRack", new { rackId = rack.bikeRackId});
        }

        public ActionResult newUser()
        {
            return View(new BikeShare.ViewModels.bikeUserPermissionViewModel());
        }

        [HttpPost]
        public ActionResult newUser([Bind] BikeShare.ViewModels.bikeUserPermissionViewModel user)
        {
            userRepo.createuser(user.userName, user.email, user.phone, user.canCheckOutBikes, user.canBorrowBikes, user.canMaintainBikes, user.canManageApp);
            return RedirectToAction("userList", "Admin");
        }

        public ActionResult newWorkshop()
        {
            return View(new Workshop());
        }

        [HttpPost]
        public ActionResult newWorkshop([Bind] Workshop shop)
        {
            workshopRepo.createNewWorkshop(shop.name, shop.GPSCoordX, shop.GPSCoordY);
            return RedirectToAction("workshopList");
        }

        public ActionResult archiveWorkshop(int workshopId)
        {
            return View(workshopRepo.getWorkshopById(workshopId));
        }

        [HttpPost]
        public ActionResult archiveWorkshop([Bind] Workshop shop)
        {
            workshopRepo.archiveWorkshopById(shop.workshopId);
            return Index();
        }

        public ActionResult bikeMaintenance(int bikeId = 1, int page = 1)
        {
            var model = new ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = repo.getMaintenanceForBike(pageSize, (page - 1) * pageSize, bikeId).ToList();
            model.pagingInfo = new ViewModels.PageInfo(500, pageSize, page);
            return View(model);
        }

        public ActionResult userDetails(int userId)
        {
            return View(repo.getUserById(userId));
        }

        public ActionResult userEdit(int userId)
        {
            return View(repo.getUserById(userId));
        }

        [HttpPost]
        public ActionResult userEdit([Bind] bikeUser user)
        {
            repo.updateUser(user);
            return RedirectToAction("userDetails", new { userId = user.bikeUserId });
        }

        public ActionResult workshopDetails(int workshopId)
        {
            return View(repo.getWorkshopById(workshopId));
        }

        public ActionResult chargesList(int page = 1)
        {
            var model = new ViewModels.PaginatedViewModel<Charge>();
            model.modelList = financeRepo.getAllCharges(pageSize, (page - 1) * pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(financeRepo.countTotalCharges(), pageSize, page);
            ViewBag.totalCharges = financeRepo.countTotalCharges();
            ViewBag.totalResolved = financeRepo.countResolvedCharges();
            ViewBag.totalPaid = financeRepo.incomeToDate();
            ViewBag.totalUnpaid = financeRepo.outstandingBalance();
            return View(model);
        }

        public ActionResult chargeDetails(int chargeId)
        {
            return View(repo.getChargeById(chargeId));
        }

        [HttpPost]
        public ActionResult closeCharge(int chargeId, decimal amountPaid)
        {
            financeRepo.closeCharge(chargeId, amountPaid);
            return View("chargeDetails", repo.getChargeById(chargeId));
        }

        [HttpPost]
        public ActionResult editCharge(int chargeId, decimal amountCharged, string chargeTitle, string chargeDescription)
        {
            financeRepo.updateCharge(chargeId, amountCharged, chargeTitle, chargeDescription);
            return RedirectToAction("chargeDetails", "Admin", new { chargeId = chargeId });
        }

        public ActionResult newCharge()
        {
            return View(new Charge());
        }
        [HttpPost]
        public ActionResult newCharge(decimal amountCharged, string chargeTitle, string chargeDescription, string chargeUser)
        {
            financeRepo.addCharge(amountCharged, chargeUser, chargeTitle, chargeDescription);
            return RedirectToAction("chargesList", "Admin");
        }
        
        public ActionResult newHour(int? workshopId, int? rackId)
        {
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
       public ActionResult newHour(int? workshopId, int? rackId, [Bind] Hour hour)
        {
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
       public ActionResult newComment(int maintId)
       {
           var model = new MaintenanceUpdate();
           model.associatedEvent = repo.getMaintenanceById(maintId);
           return View(model);
       }
       [HttpPost]
       public ActionResult newComment(int maintenanceId, string commentTitle, string commentBody)
       {
           maintRepo.commentOnMaint(maintenanceId, commentTitle, commentBody, User.Identity.Name);
           return RedirectToAction("MaintenanceDetails", new { maintId = maintenanceId });
       }
       public ActionResult maintenanceDetails(int maintId)
       {
           return View(maintRepo.getMaintenanceById(maintId));
       }

       public ActionResult uploadImage(int rackId)
       {
           return View(repo.getRackById(rackId));
       }

       [HttpPost]
       public ActionResult uploadImage(int rackId, string image)
       {
           HttpPostedFileBase file = Request.Files["image"];
           byte[] tempImage = new byte[file.ContentLength];
           file.InputStream.Read(tempImage, 0, file.ContentLength);
           file.SaveAs(Request.PhysicalApplicationPath.ToString() + "\\Content\\Images\\Racks\\" + rackId + ".jpg");
           return RedirectToAction("editRack", new { rackId = rackId });
       }

       public ActionResult doesUserExist(string userName)
       {
           var x = Json(userRepo.doesUserExist(userName), JsonRequestBehavior.AllowGet);
            return x;

       }
       [HttpPost]
       public ActionResult hourDelete(int? workshopId, int? rackId, int hourId)
       {
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
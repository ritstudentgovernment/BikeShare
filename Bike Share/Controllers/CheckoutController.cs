using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Models;
using BikeShare.Interfaces;

namespace BikeShare.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private ICheckOutRepository repo;
        private IAdminRepository adminRepo;
        private IUserRepository userRepo;
        private IFinanceRepository financeRepo;
        private IMaintenanceRepository maintRepo;

        private bool authorize()
        {
            if (!userRepo.getUserByName(User.Identity.Name).canCheckOutBikes)
            {
                return false;
            }
            return true;
        }

        public CheckoutController(ICheckOutRepository checkParam, IFinanceRepository fParam, IUserRepository uRepo,IAdminRepository aRepo, IMaintenanceRepository mRepo)
        {
            repo = checkParam;
            financeRepo = fParam;
            userRepo = uRepo;
            adminRepo = aRepo;
            maintRepo = mRepo;
        }

        // GET: Checkout
        public ActionResult Index(int rackId, string message = "")
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.CheckoutViewModel();
            model.availableBikes = repo.getAvailableBikesForRack(rackId);
            model.checkedOutBikes = repo.getCheckedOutBikes();
            model.unavailableBikes = repo.getUnavailableBikesForRack(rackId);
            model.errorMessage = message;

            foreach(Bike bike in model.checkedOutBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
                if (bike.checkOuts.Count < 1)
                {
                    CheckOut defaultCheckOut = new CheckOut();
                    defaultCheckOut.user = new bikeUser();
                    defaultCheckOut.user.userName = "none";
                    bike.checkOuts.Add(defaultCheckOut);
                }
            }
            foreach (Bike bike in model.availableBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
                if (bike.checkOuts.Count < 1)
                {
                    CheckOut defaultCheckOut = new CheckOut();
                    defaultCheckOut.user = new bikeUser();
                    defaultCheckOut.user.userName = "none";
                    bike.checkOuts.Add(defaultCheckOut);
                }
            }

            model.currentRack = repo.getRackById(rackId);
            model.checkOutPerson = User.Identity.Name;
            return View(model);
        }

        public ActionResult selectRack()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getAllRacks());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult checkOutBike(int rackId, [Bind]BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var errorMessage = repo.checkOutBike(model.selectedBikeForCheckout, model.userToCheckIn, User.Identity.Name, model.currentRack.bikeRackId);
            return RedirectToAction("Index", new { rackId = rackId, message = errorMessage });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult checkInBike(int rackId, [Bind] BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            var errorMessage = repo.checkInBike(model.selectedBikeForCheckout, User.Identity.Name, model.currentRack.bikeRackId);
            return RedirectToAction("Index", new { rackId = rackId, message = errorMessage });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitCharge(string userName, string chargeTitle, string chargeDetails, int rackId, decimal chargeAmount)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            financeRepo.addCharge(chargeAmount, userName, chargeTitle, chargeDetails);
            return RedirectToAction("Index", new { rackId = rackId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitMaint(string maintTitle, string maintDetails, int rackId, int bikeId, string disableBike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            bool bikeDisabled = true; if (String.IsNullOrEmpty(disableBike)) { bikeDisabled = false; }
            maintRepo.newMaintenance(bikeId, maintTitle, maintDetails, User.Identity.Name, maintRepo.getAllWorkshops().First().workshopId, bikeDisabled);
            return RedirectToAction("Index", new { rackId = rackId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitChargeAndMaint(string userName, string title, string details, int rackId, decimal chargeAmount, int bikeId, string disableBike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            financeRepo.addCharge(chargeAmount, userName, title, details);
            bool bikeDisabled = true; if (String.IsNullOrEmpty(disableBike)) { bikeDisabled = false; }
            maintRepo.newMaintenance(bikeId, title, details, User.Identity.Name, maintRepo.getAllWorkshops().First().workshopId, bikeDisabled);
            return RedirectToAction("Index", new { rackId = rackId });
        }
        public ActionResult doesUserExist(string validationName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return Json(userRepo.doesUserExist(validationName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult isUserValid(string validationName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            if(userRepo.doesUserExist(validationName))
            {
                return Json(userRepo.isUserRegistrationValid(userRepo.getUserByName(validationName).bikeUserId), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult helpPopup()
        {
            return View();
        }

        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult { Data = "Success" };
        }
    }
}
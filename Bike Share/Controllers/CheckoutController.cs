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

        private bool authorize()
        {
            if (!userRepo.getUserByName(User.Identity.Name).canCheckOutBikes)
            {
                return false;
            }
            return true;
        }

        public CheckoutController(ICheckOutRepository checkParam, IFinanceRepository fParam, IUserRepository uRepo,IAdminRepository aRepo)
        {
            repo = checkParam;
            financeRepo = fParam;
            userRepo = uRepo;
            adminRepo = aRepo;
        }

        // GET: Checkout
        public ActionResult Index(int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.CheckoutViewModel();
            model.availableBikes = repo.getAvailableBikesForRack(rackId);
            model.checkedOutBikes = repo.getCheckedOutBikes();
            foreach(Bike bike in model.checkedOutBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
            }
            foreach (Bike bike in model.availableBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
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
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.errorMessage = repo.checkOutBike(model.selectedBikeForCheckout, model.userToCheckIn, User.Identity.Name, model.currentRack.bikeRackId);
            newModel.availableBikes = repo.getAvailableBikesForRack(model.currentRack.bikeRackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            foreach (Bike bike in newModel.checkedOutBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
            }
            foreach (Bike bike in newModel.availableBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
            }
            newModel.currentRack = repo.getRackById(model.currentRack.bikeRackId);
            return View("Index", newModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult checkInBike(int rackId, [Bind] BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.errorMessage = repo.checkInBike(model.selectedBikeForCheckout, User.Identity.Name, model.currentRack.bikeRackId);
            newModel.availableBikes = repo.getAvailableBikesForRack(model.currentRack.bikeRackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            foreach (Bike bike in newModel.checkedOutBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
            }
            foreach (Bike bike in newModel.availableBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
            }
            newModel.currentRack = repo.getRackById(model.currentRack.bikeRackId);
            return View("Index", newModel );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitCharge(string userName, string chargeTitle, string chargeDetails, int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            financeRepo.addCharge((decimal)5, userName, chargeTitle, chargeDetails);
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.availableBikes = repo.getAvailableBikesForRack(rackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            foreach (Bike bike in newModel.checkedOutBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
            }
            foreach (Bike bike in newModel.availableBikes)
            {
                bike.checkOuts = adminRepo.getBikesCheckouts(bike.bikeId, 1, 0).ToList();
            }
            newModel.currentRack = repo.getRackById(rackId);
            return View("Index", newModel);
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
                return Json(userRepo.doesUserExist(validationName), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult helpPopup()
        {
            return View();
        }
    }
}
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

        public CheckoutController(ICheckOutRepository checkParam, IFinanceRepository fParam, IUserRepository uRepo)
        {
            repo = checkParam;
            financeRepo = fParam;
            userRepo = uRepo;
        }

        // GET: Checkout
        [BikeShare.Code.UserNameFilter]
        public ActionResult Index(int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.CheckoutViewModel();
            model.availableBikes = repo.getAvailableBikesForRack(rackId);
            model.checkedOutBikes = repo.getCheckedOutBikes();
            model.currentRack = repo.getRackById(rackId);
            model.checkOutPerson = User.Identity.Name;
            return View(model);
        }

        [BikeShare.Code.UserNameFilter]
        public ActionResult selectRack()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(repo.getAllRacks());
        }

        [HttpPost]
        [BikeShare.Code.UserNameFilter]
        public ActionResult checkOutBike(int rackId, [Bind]BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.errorMessage = repo.checkOutBike(model.selectedBikeForCheckout, model.userToCheckIn, User.Identity.Name, model.currentRack.bikeRackId);
            newModel.availableBikes = repo.getAvailableBikesForRack(model.currentRack.bikeRackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            newModel.currentRack = repo.getRackById(model.currentRack.bikeRackId);
            return View("Index", newModel);
        }

        [HttpPost]
        [BikeShare.Code.UserNameFilter]
        public ActionResult checkInBike(int rackId, [Bind] BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.errorMessage = repo.checkInBike(model.selectedBikeForCheckout, User.Identity.Name, model.currentRack.bikeRackId);
            newModel.availableBikes = repo.getAvailableBikesForRack(model.currentRack.bikeRackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            newModel.currentRack = repo.getRackById(model.currentRack.bikeRackId);
            return View("Index", newModel );
        }

        [HttpPost]
        [BikeShare.Code.UserNameFilter]
        public ActionResult submitCharge(string userName, string chargeTitle, string chargeDetails, int rackId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            financeRepo.addCharge((decimal)5, userName, chargeTitle, chargeDetails);
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.availableBikes = repo.getAvailableBikesForRack(rackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            newModel.currentRack = repo.getRackById(rackId);
            return View("Index", newModel);
        }
    }
}
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
        private IFinanceRepository financeRepo;
        public CheckoutController(ICheckOutRepository checkParam, IFinanceRepository fParam)
        {
            repo = checkParam;
            financeRepo = fParam;
        }

        // GET: Checkout
        public ActionResult Index(int rackId)
        {
            var model = new BikeShare.ViewModels.CheckoutViewModel();
            model.availableBikes = repo.getAvailableBikesForRack(rackId);
            model.checkedOutBikes = repo.getCheckedOutBikes();
            model.currentRack = repo.getRackById(rackId);
            model.checkOutPerson = User.Identity.Name;
            return View(model);
        }

        public ActionResult selectRack()
        {
            return View(repo.getAllRacks());
        }

        [HttpPost]
        public ActionResult checkOutBike(int rackId, [Bind]BikeShare.ViewModels.CheckoutViewModel model)
        {
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.errorMessage = repo.checkOutBike(model.selectedBikeForCheckout, model.userToCheckIn, User.Identity.Name, model.currentRack.bikeRackId);
            newModel.availableBikes = repo.getAvailableBikesForRack(model.currentRack.bikeRackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            newModel.currentRack = repo.getRackById(model.currentRack.bikeRackId);
            Response.RedirectToRoute(new { action = "Index", controller = "CheckOut", rackId = model.currentRack.bikeRackId });
            return View("Index", newModel);
        }

        [HttpPost]
        public ActionResult checkInBike(int rackId, [Bind] BikeShare.ViewModels.CheckoutViewModel model)
        {
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.errorMessage = repo.checkInBike(model.selectedBikeForCheckout, User.Identity.Name, model.currentRack.bikeRackId);
            newModel.availableBikes = repo.getAvailableBikesForRack(model.currentRack.bikeRackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            newModel.currentRack = repo.getRackById(model.currentRack.bikeRackId);
            return View("Index", newModel );
        }

        [HttpPost]
        public ActionResult submitCharge(string userName, string chargeTitle, string chargeDetails, int rackId)
        {
            financeRepo.addCharge((decimal)5, userName, chargeTitle, chargeDetails);
            var newModel = new BikeShare.ViewModels.CheckoutViewModel();
            newModel.availableBikes = repo.getAvailableBikesForRack(rackId);
            newModel.checkedOutBikes = repo.getCheckedOutBikes();
            newModel.currentRack = repo.getRackById(rackId);
            return View("Index", newModel);
        }
    }
}
using BikeShare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BikeShare.Code;

namespace BikeShare.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private BikesContext context;

        private bool authorize()
        {
            try
            {
                return context.BikeUser.Where(n => n.userName == User.Identity.Name).First().canCheckOutBikes;
            }
            catch
            {
                return false;
            }
        }

        public CheckoutController()
        {
            context = new BikesContext();
        }

        // GET: Checkout
        public ActionResult Index(int rackId, string message = "")
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.CheckoutViewModel();
            
            model.currentRack = context.BikeRack.Find(rackId);
            model.errorMessage = message;
            model.userToCheckIn = null;
            model.checkOutPerson = User.Identity.Name;
            var baseBikeList = context.Bike.Where(b => !b.isArchived).Where(r => r.bikeRackId.Value == rackId || !r.bikeRackId.HasValue).ToList();
            model.availableBikes = baseBikeList.Where(i => i.isAvailable() && i.bikeRackId.HasValue);
            model.unavailableBikes = baseBikeList.Where(i => !i.isAvailable());
            model.checkedOutBikes = baseBikeList.Where(r => !r.bikeRackId.HasValue);
            model.lastCheckoutUserForBike = new Dictionary<int, string>();
            model.lastCheckoutTimeForBike = new Dictionary<int, string>();
            var listBikeIds = baseBikeList.Select(i => i.bikeId).ToList();
            var checkouts = context.CheckOut.Where(c=> listBikeIds.Contains(c.bike)).ToList().OrderByDescending(t => t.timeOut).ToList();
            foreach(var bike in baseBikeList)
            {
                try
                {
                    var first = checkouts.Where(b => b.bike == bike.bikeId).First();
                    checkouts = checkouts.Where(b => b.bike != bike.bikeId).ToList();
                    checkouts.Add(first);
                }
                catch
                {
                    model.lastCheckoutTimeForBike.Add(bike.bikeId, "no data");
                    model.lastCheckoutUserForBike.Add(bike.bikeId, "no data");
                }
            }
            foreach(var checkout in checkouts)
            {
                int lastPerson = checkout.rider;
                try
                {


                    model.lastCheckoutUserForBike.Add(checkout.bike, context.BikeUser.Find(lastPerson).userName);
                    if (checkout.timeIn.HasValue)
                    {
                        model.lastCheckoutTimeForBike.Add(checkout.bike, checkout.timeIn.ToString());
                    }
                    else
                    {
                        model.lastCheckoutTimeForBike.Add(checkout.bike, checkout.timeOut.ToString());
                    }
                }
                catch(NullReferenceException)
                {
                    model.lastCheckoutTimeForBike.Add(checkout.bike, "no data");
                    model.lastCheckoutUserForBike.Add(checkout.bike, "no data");
                }
            }

            return View(model);
        }

        public ActionResult selectRack()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.BikeRack.Where(a => !a.isArchived));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult checkOutBike(int rackId, [Bind]BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            try
            {
                Bike bike = context.Bike.Find(model.selectedBikeForCheckout);
                bike.lastCheckedOut = DateTime.Now;
                CheckOut check = new CheckOut
                {
                    bike = bike.bikeId,
                    isResolved = false,
                    rackCheckedOut = rackId,
                    timeOut = DateTime.Now,
                };

                bikeUser rider = context.BikeUser.Where(n => n.userName == model.userToCheckIn).First();
                bikeUser dcheckOutPerson = context.BikeUser.Where(n => n.userName == User.Identity.Name).First();
                if (!rider.canBorrowBikes)
                {
                    return RedirectToAction("Index", new { rackId = rackId, message = "The user doesn't have riding privileges." });
                }
                if (rider.lastRegistered.AddDays(context.settings.First().daysBetweenRegistrations) < DateTime.Now)
                {
                    return RedirectToAction("Index", new { rackId = rackId, message = "The user's registration is out of date. Please remind them to register." });
                }
                bike.bikeRackId = null;
                check.rider = rider.bikeUserId;
                check.checkOutPerson = dcheckOutPerson.bikeUserId;
                context.CheckOut.Add(check);
                context.SaveChanges();
                Mailing.queueCheckoutNotice(rider.email, DateTime.Now.AddHours(24));
            }
            catch (System.InvalidOperationException)
            {
                return RedirectToAction("Index", new { rackId = rackId, message = "That user isn't in the system." });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException)
            {
                return RedirectToAction("Index", new { rackId = rackId, message = "Checkout didn't validate. Sorry." });
            }
            return RedirectToAction("Index", new { rackId = rackId, message = "Checkout successful!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult checkInBike(int rackId, [Bind] BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            try
            {
                var checkout = context.CheckOut.Where(t => !t.timeIn.HasValue).Where(b => b.bike == model.selectedBikeForCheckout).First();
                checkout.isResolved = true;
                checkout.timeIn = DateTime.Now;
                var bike = context.Bike.Find(model.selectedBikeForCheckout);
                bike.bikeRackId = rackId;
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException)
            {
                return RedirectToAction("Index", new { rackId = rackId, message = "There was an issue checking the bike in." });
            }
            return RedirectToAction("Index", new { rackId = rackId, message = "Bike successfully checked in." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitCharge(string userName, string chargeTitle, string chargeDetails, int rackId, decimal chargeAmount)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            Charge charge = new Charge
            {
                amountCharged = chargeAmount,
                dateAssesed = DateTime.Now,
                dateResolved = DateTime.Now,
                title = chargeTitle,
                description = chargeDetails,
                user = context.BikeUser.Where(u => u.userName == userName).First()
            };
            context.Charge.Add(charge);
            context.SaveChanges();
            return RedirectToAction("Index", new { rackId = rackId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitMaint(string maintTitle, string maintDetails, int rackId, int bikeId, string disableBike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var maintenance = new MaintenanceEvent { timeAdded = DateTime.Now, timeResolved = DateTime.Now, title = maintTitle, details = maintDetails, disableBike = !String.IsNullOrEmpty(disableBike) };
            maintenance.submittedById = context.BikeUser.Where(u => u.userName == User.Identity.Name).First().bikeUserId;
            context.MaintenanceEvent.Add(maintenance);
            context.SaveChanges();
            return RedirectToAction("Index", new { rackId = rackId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult submitChargeAndMaint(string userName, string title, string details, int rackId, decimal chargeAmount, int bikeId, string disableBike)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            Charge charge = new Charge
            {
                amountCharged = chargeAmount,
                dateAssesed = DateTime.Now,
                dateResolved = DateTime.Now,
                title = title,
                description = details,
                user = context.BikeUser.Where(u => u.userName == userName).First()
            };
            context.Charge.Add(charge);
            var maintenance = new MaintenanceEvent { timeAdded = DateTime.Now, timeResolved = DateTime.Now, title = title, details = details, disableBike = !String.IsNullOrEmpty(disableBike) };
            maintenance.submittedById = context.BikeUser.Where(u => u.userName == User.Identity.Name).First().bikeUserId;
            context.MaintenanceEvent.Add(maintenance);
            context.SaveChanges();
            return RedirectToAction("Index", new { rackId = rackId });
        }

        public ActionResult doesUserExist(string validationName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return Json(context.BikeUser.Where(u => u.userName == validationName).Count() > 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult isUserValid(string validationName)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            if (context.BikeUser.Where(u => u.userName == validationName).Where(i => !i.isArchived).Count() > 0)
            {
                var user = context.BikeUser.Where(u => u.userName == validationName).First();
                return Json(user.canBorrowBikes && user.lastRegistered.AddDays(context.settings.First().daysBetweenRegistrations) > DateTime.Now, JsonRequestBehavior.AllowGet);
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
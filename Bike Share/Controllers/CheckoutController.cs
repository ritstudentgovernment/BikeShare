using BikeShare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

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
            var result = new List<Bike>();
            var query = context.Bike.Where(c => c.checkOuts.All(r => r.isResolved)).Where(r => r.bikeRack.bikeRackId == rackId).Where(a => !a.isArchived);
            var maint = context.MaintenanceEvent.Where(r => !r.resolved).Where(d => d.disableBike).Select(b => b.bikeAffected.bikeId);
            var spec = context.Inspection.Where(b => b.bike.bikeRack.bikeRackId == rackId);
            var inspectionPeriod = (int)context.settings.First().DaysBetweenInspections;
            List<Bike> temp = query.Where(b => !maint.Contains(b.bikeId)).ToList().Where(l => { DateTime x = (DateTime)l.lastPassedInspection; return x.AddDays(inspectionPeriod) > DateTime.Now; }).ToList();
            foreach (var bike in temp)
            {
                if (spec.Where(b => b.bike.bikeId == bike.bikeId).OrderByDescending(d => d.datePerformed).First().isPassed == true)
                {
                    result.Add(bike);
                }
            }
            model.availableBikes = result;
            model.checkedOutBikes = context.CheckOut.Where(i => !i.isResolved).Select(b => b.bike);
            model.unavailableBikes = context.BikeRack.Include(b => b.bikes).Where(r => r.bikeRackId == rackId).First().bikes.Except(model.availableBikes);
            model.errorMessage = message;

            foreach (Bike bike in model.checkedOutBikes.Union(model.availableBikes).ToList())
            {
                bike.checkOuts = context.Bike.Find(bike.bikeId).checkOuts.OrderByDescending(d => d.timeOut).ToList();
                if (bike.checkOuts.Count < 1)
                {
                    CheckOut defaultCheckOut = new CheckOut();
                    defaultCheckOut.user = new bikeUser();
                    defaultCheckOut.user.userName = "none";
                    bike.checkOuts.Add(defaultCheckOut);
                }
            }
            model.currentRack = context.BikeRack.Find(rackId);
            model.checkOutPerson = User.Identity.Name;
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
                bike.bikeRack = context.BikeRack.Find(rackId); ;
                bike.lastCheckedOut = DateTime.Now;
                CheckOut check = new CheckOut
                {
                    bike = bike,
                    isResolved = false,
                    rackCheckedOut = bike.bikeRack,
                    timeOut = DateTime.Now,
                    timeIn = DateTime.Now
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
                check.user = rider;
                check.checkOutPerson = dcheckOutPerson;
                rider.bike = bike;
                rider.hasBike = true;
                rider.checkOuts.Add(check);
                context.CheckOut.Add(check);
                var trace = new Tracer();
                trace.checkOut = check;
                trace.user = check.user;
                trace.time = DateTime.Now;
                trace.comment = "User Checked out Bike";
                context.tracer.Add(trace);
                context.SaveChanges();
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtpServer = new SmtpClient();
                    mail.To.Add(rider.email);
                    mail.Subject = "Bike Checked Out";
                    mail.Body = "Thank you for checking out a bike! You have the bike for 24 hours. Enjoy your ride and be safe!";
                    if (DateTime.Now.ToString("ddd") == "Fri")
                    {
                        mail.Body = "Thank you for checking out a bike! You have the bike until Monday morning. Enjoy your ride and be safe!";
                    }
                    smtpServer.Send(mail);
                }
                catch
                {
                    return RedirectToAction("Index", new { rackId = rackId, message = "The checkout was succesful, but there was difficulty sending email. Please let the system administrator know." });
                }
            }
            catch (System.InvalidOperationException)
            {
                return RedirectToAction("Index", new { rackId = rackId, message = "That user isn't in the system." });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException)
            {
                return RedirectToAction("Index", new { rackId = rackId, message = "Checkout didn't validate. Sorry." });
            }
            return RedirectToAction("Index", new { rackId = rackId, message = "Checkout succesful!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult checkInBike(int rackId, [Bind] BikeShare.ViewModels.CheckoutViewModel model)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            try
            {
                Bike bike = context.Bike.Find(model.selectedBikeForCheckout);
                bike.bikeRack = context.BikeRack.Find(rackId);
                CheckOut check = context.CheckOut.Where(r => !r.isResolved).Where(b => b.bike.bikeId == model.selectedBikeForCheckout).First();
                check.bike = bike;
                check.isResolved = true;
                check.rackCheckedIn = bike.bikeRack;
                check.timeIn = DateTime.Now;
                bikeUser rider = check.user;
                rider.bike = null;
                rider.hasBike = false;
                var trace = new Tracer();
                trace.checkOut = check;
                trace.user = check.user;
                trace.time = DateTime.Now;
                trace.comment = "User Checked out Bike";
                context.tracer.Add(trace);
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException)
            {
                return RedirectToAction("Index", new { rackId = rackId, message = "There was an issue checking the bike in." });
            }
            return RedirectToAction("Index", new { rackId = rackId, message = "Bike succesfully checked in." });
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
            maintenance.staffPerson = context.BikeUser.Where(u => u.userName == User.Identity.Name).First();
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
            maintenance.staffPerson = context.BikeUser.Where(u => u.userName == User.Identity.Name).First();
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
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
        public ActionResult Index()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var model = new ViewModels.dashViewModel();
            model.countBikes = context.Bike.Where(b => !b.isArchived).Count();
            model.countAvailableBikes = context.Bike.Where(b => !b.isArchived).ToList().Where(b => b.isAvailable()).Count();
            model.countCharges = context.Charge.Count();
            model.countCheckouts = context.CheckOut.Count();
            model.countMaintenance = context.MaintenanceEvent.Count();
            model.countOngoingMaintenance = context.MaintenanceEvent.Where(m => m.timeResolved == null).Count();
            model.countRacks = context.BikeRack.Where(r => !r.isArchived).Count();
            model.countInspections = context.Inspection.Count();
            model.countUsers = context.BikeUser.Where(a => !a.isArchived).Count();
            return View(model);
        }

        /// <summary>
        /// Displays the new bike form
        /// </summary>
        /// <returns></returns>
        public ActionResult newBike()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            context.Bike.Add(new Bike(bikeModel.bikeNumber)
            {
                bikeRackId = bikeModel.bikeRackId,
            }); 
            context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        /// <summary>
        /// Displays the form for creating a new bike rack.
        /// </summary>
        /// <returns></returns>
        public ActionResult newRack()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            context.Bike.Find(bike.bikeId).isArchived = !context.Bike.Find(bike.bikeId).isArchived;
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            context.BikeRack.Find(rack.bikeRackId).isArchived = !context.BikeRack.Find(rack.bikeRackId).isArchived;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult archiveUser(int userId)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(context.BikeUser.Find(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult archiveUser([Bind(Include = "bikeUserId")] bikeUser user)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            context.BikeUser.Find(user.bikeUserId).isArchived = !context.BikeUser.Find(user.bikeUserId).isArchived;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult appSettings()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(context.settings.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult appSettings([Bind] appSetting settings)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            context.settings.Remove(context.settings.First());
            context.settings.Add(settings);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult bikeList(int? rackId, int page = 1, bool incMissing = true, bool incOverdue = true, bool incCheckedOut = true, bool incCheckedIn = true, bool incCurrent = true, bool incArchived = false)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var model = new ViewModels.FilteredBikeViewModel();
            var all = new List<Bike>();
            if (incCurrent)
            {
                all.AddRange(context.Bike.Where(a => !a.isArchived).ToList());
            }
            if (incArchived)
            {
                all.AddRange(context.Bike.Where(a => a.isArchived).ToList());
            }
            if (!incCheckedIn)
            {
                all = all.Where(r => r.bikeRackId == null).ToList();
            }
            if (!incCheckedOut)
            {
                all = all.Where(r => r.bikeRackId != null).ToList();
            }
            if (rackId != null)
            {
                all = all.Where(r => r.bikeRackId == rackId).ToList();
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var model = new ViewModels.rackListingViewModel();
            model.bikeRacks = context.BikeRack.OrderByDescending(d => d.bikeRackId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.countBikeRacks = model.bikeRacks.Count();
            model.pagingInfo = new ViewModels.PageInfo(context.BikeRack.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult userList(string name = "", int page = 1, bool? hasCharges = null, bool? hasBike = null, bool? canMaintain = null, bool? canAdmin = null, bool? canRide = null, bool? canCheckout = null)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
           
            var model = new ViewModels.PaginatedViewModel<bikeUser>();
            model.modelList = new List<bikeUser>();
            if(hasCharges.HasValue && hasCharges.Value)
            {
                model.modelList = context.Charge.Where(r => !r.isResolved).Select(u => u.user).ToList();
            }
            if(hasBike.HasValue && hasBike.Value)
            {
                var userIds = context.CheckOut.Where(r => !r.isResolved).Select(u => u.rider).ToList();
                userIds.ForEach(u => model.modelList.Add(context.BikeUser.Find(u)));
            }
            
            if(canMaintain.HasValue && canMaintain.Value)
            {
                model.modelList = context.BikeUser.Where(c => c.canMaintainBikes).ToList();
            }
            if(canAdmin.HasValue && canAdmin.Value)
            {
                model.modelList = context.BikeUser.Where(c => c.canAdministerSite).ToList();
            }
            if(canRide.HasValue && canRide.Value)
            {
                model.modelList = context.BikeUser.Where(c => !c.canBorrowBikes).ToList();
            }
            if(canCheckout.HasValue && canCheckout.Value)
            {
                model.modelList = context.BikeUser.Where(c => c.canCheckOutBikes).ToList();
            }
            if(!String.IsNullOrWhiteSpace(name))
            {
                model.modelList = context.BikeUser.Where(c => c.userName.Contains(name)).ToList();
                model.modelList.AddRange(context.BikeUser.Where(c => c.firstName.Contains(name)).ToList());
                model.modelList.AddRange(context.BikeUser.Where(c => c.lastName.Contains(name)).ToList());
            }
            int totalResults = model.modelList.Count();
            model.modelList = model.modelList.OrderByDescending(d => d.userName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            //ViewBag.canRide = canRide; ViewBag.canMaintain = canMaintain; ViewBag.canAdmin = canAdmin; ViewBag.canCheckout = canCheckout;
            model.pagingInfo = new ViewModels.PageInfo(totalResults, pageSize, page);
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var model = new ViewModels.PaginatedViewModel<CheckOut>();

            IQueryable<CheckOut> list;
            if (bikeID == null && rackId == null)
            {
                list = context.CheckOut;
            }
            else if (rackId != null)
            {
                list = context.CheckOut.Where(u => u.rackCheckedOut == rackId || u.rackCheckedIn == rackId);
            }
            else
            {
                list = context.CheckOut.Where(b => b.bike == bikeID);
            }
            int total = list.Count();
            model.modelList = list.OrderByDescending(d => d.timeOut).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(total, pageSize, page);
            return View(model);
        }

        public ActionResult editBike(int bikeId)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
           
            return View(context.Bike.Find(bikeId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editBike([Bind] Bike bike)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
           
            var dbike = context.Bike.Find(bike.bikeId);
            dbike.bikeName = bike.bikeName;
            dbike.bikeNumber = bike.bikeNumber;
            dbike.isArchived = bike.isArchived;
            context.SaveChanges();

            return RedirectToAction("bikeList");
        }

        public ActionResult infoBike(int bikeID)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var model = new BikeShare.ViewModels.superBike();

            model.bike = context.Bike.Find(bikeID);
            model.inspections = context.Inspection.Where(b => b.bikeId == bikeID).OrderByDescending(d => d.datePerformed).Take(25).ToList();
            model.maintenance = context.MaintenanceEvent.Where(b => b.bikeId == bikeID).OrderByDescending(d => d.timeAdded).Take(25).ToList();
            return View(model);
        }

        public ActionResult editRack(int rackId)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
           
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
           
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(new BikeShare.ViewModels.bikeUserPermissionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newUser([Bind] BikeShare.ViewModels.bikeUserPermissionViewModel user)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
           
            context.BikeUser.Add(new bikeUser
            {
                userName = user.userName,
                phoneNumber = user.phone,
                isArchived = false,
                email = user.email,
                canMaintainBikes = user.canMaintainBikes,
                canCheckOutBikes = user.canCheckOutBikes,
                canAdministerSite = user.canManageApp,
                canBorrowBikes = user.canBorrowBikes
            });
            context.SaveChanges();

            return RedirectToAction("userList", "Admin");
        }

       

        public ActionResult userDetails(int userId)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(context.BikeUser.Find(userId));
        }

        public ActionResult userEdit(int userId)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(context.BikeUser.Find(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult userEdit([Bind] bikeUser user)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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

     
        public ActionResult chargesList(int page = 1)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var model = new ViewModels.PaginatedViewModel<Charge>();
            model.modelList = context.Charge.OrderByDescending(d => d.chargeId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(context.Charge.Count(), pageSize, page);
            ViewBag.totalCharges = context.Charge.Count();
            
            if(context.Charge.Count() > 0)
            {
                ViewBag.totalPaid = context.Charge.Where(c => c.amountPaid != null).Sum(c => c.amountPaid);
                ViewBag.totalResolved = context.Charge.Where(i => i.isResolved).Count();
                ViewBag.totalUnpaid = context.Charge.Sum(c => c.amountCharged) - ViewBag.totalPaid;
            }
            else
            {
                ViewBag.totalPaid = 0;
                ViewBag.totalResolved = 0;
                ViewBag.totalUnpaid = 0;
            }
            return View(model);
        }

        public ActionResult chargeDetails(int chargeId)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(context.Charge.Find(chargeId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult closeCharge(int chargeId, decimal amountPaid)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var x = context.Charge.Find(chargeId);
            x.amountCharged = amountCharged;
            x.title = chargeTitle;
            x.description = chargeDescription;
            context.SaveChanges();
            return RedirectToAction("chargeDetails", "Admin", new { chargeId = chargeId });
        }

        public ActionResult newCharge(string userName)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(new Charge());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newCharge(decimal amountCharged, string chargeTitle, string chargeDescription, string chargeUser)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            context.Charge.Add(new Charge { amountCharged = amountCharged, dateResolved = DateTime.Now, title = chargeTitle, dateAssesed = DateTime.Now, description = chargeDescription, user = context.BikeUser.Where(u => u.userName == chargeUser).First() });
            context.SaveChanges();
            return RedirectToAction("chargesList", "Admin");
        }

        public ActionResult uploadImage(int rackId)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(context.BikeRack.Find(rackId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult uploadImage(int rackId, string image)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
            HttpPostedFileBase file = Request.Files["image"];
            byte[] tempImage = new byte[file.ContentLength];
            file.InputStream.Read(tempImage, 0, file.ContentLength);
            file.SaveAs(Request.PhysicalApplicationPath.ToString() + "\\Content\\Images\\Racks\\" + rackId + ".jpg");
            return RedirectToAction("editRack", new { rackId = rackId });
        }

        public ActionResult uploadWaiver()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View();
        }

        [HttpPost]
        public ActionResult uploadWaiver(string waiver)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            HttpPostedFileBase file = Request.Files["waiver"];
            byte[] tempWaiver = new byte[file.ContentLength];
            file.InputStream.Read(tempWaiver, 0, file.ContentLength);
            int newNumber = context.settings.First().latestPDFNumber.GetValueOrDefault(0) + 1;
            file.SaveAs(Request.PhysicalApplicationPath.ToString() + "\\Content\\waivers\\" + newNumber + ".pdf");
            context.settings.First().latestPDFNumber = newNumber;
            context.SaveChanges();
            return RedirectToAction("appSettings");
        }

        public ActionResult doesUserExist(string validationName)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
            var x = Json(context.BikeUser.Where(u => u.userName == validationName).Count() > 0, JsonRequestBehavior.AllowGet);
            return x;
        }

        public ActionResult reports()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); } 
            return View();
        }

        public ActionResult schedules()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            return View(context.schedules.ToList());
        }

        [HttpPost]
        public ActionResult newSchedule(ScheduledInspection sched)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            context.schedules.Add(sched);
            context.SaveChanges();
            return RedirectToAction("schedules");
        }

        [HttpPost]
        public ActionResult deleteSchedule(int id)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            var sched = context.schedules.Find(id);
            context.schedules.Remove(sched);
            context.SaveChanges();
            return RedirectToAction("schedules");
        }

        public ActionResult resetRegistrations()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }

            return View();
        }

        [HttpPost]
        public ActionResult resetRegistrations(bool notify)
        {
            //TODO - implement notification functionality
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            foreach(var user in context.BikeUser)
            {
                user.lastRegistered = DateTime.MinValue;
            }
            context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult bulkOfflineBikes(int start, int end)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            foreach(var bike in context.Bike)
            {
                if (bike.bikeNumber >= start && bike.bikeNumber <= end)
                {
                    bike.onInspectionHold = true;
                }
            }
            context.SaveChanges();
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult checkoutReport(DateTime start, DateTime end)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            DisplayLogFile(generateCheckoutLog(start, end));

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult bikeReport()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            DisplayLogFile(generateBikeLog());

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult rackReport()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            DisplayLogFile(generateRackLog());

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult userReport()
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            DisplayLogFile(generateUserLog());

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult inspectionReport(DateTime start, DateTime end)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
            DisplayLogFile(generateInspectionLog(start, end));

            return RedirectToAction("reports");
        }

        [HttpPost]
        public ActionResult maintReport(DateTime start, DateTime end)
        {
            if (!authorize()) { return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized); }
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
            foreach (var checkout in context.CheckOut.ToList())
            {
                csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    context.BikeUser.Find(checkout.rider).userName, checkout.timeOut, checkout.timeIn, checkout.rackCheckedOut, checkout.rackCheckedIn, checkout.isResolved, context.Bike.Find(checkout.bike)));
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
            foreach (var bike in context.Bike.ToList())
            {
                csvExport.AppendLine(
                    string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                    bike.bikeNumber, bike.bikeName, bike.isArchived, bike.lastCheckedOut.ToString(), context.Inspection.Where(u => u.bikeId == bike.bikeId).Where(i => i.isPassed).OrderByDescending(d => d.datePerformed).First().datePerformed.ToString(), context.Inspection.Where(b => b.bikeId == bike.bikeId).Count()));
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
            foreach (var Inspection in context.Inspection.OrderByDescending(d => d.datePerformed).ToList())
            {
                var bike = context.Bike.Find(Inspection.bikeId);
                csvExport.AppendLine(
                string.Format(
                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                bike.bikeName, bike.bikeNumber, Inspection.datePerformed, Inspection.comment, Inspection.isPassed));
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
            foreach (var Maint in context.MaintenanceEvent.OrderByDescending(d => d.timeResolved).ToList())
            {
                csvExport.AppendLine(
                string.Format(
                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                 context.Bike.Find(Maint.bikeId).bikeNumber, Maint.title, Maint.timeAdded, Maint.timeResolved, Maint.timeResolved != null, Maint.isArchived, Maint.disableBike, Maint.details));
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
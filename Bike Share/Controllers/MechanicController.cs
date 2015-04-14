using BikeShare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BikeShare.Controllers
{
    [Authorize]
    public class MechanicController : Controller
    {
        private int pageSize = 25;
        private BikesContext context;

        private bool authorize()
        {
            try
            {
                return context.BikeUser.Where(n => n.userName == User.Identity.Name).First().canMaintainBikes;
            }
            catch
            {
                return false;
            }
        }

        public MechanicController()
        {
            context = new BikesContext();
        }

        /// <summary>
        /// Displays the mechanic's home page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.Maint.dashboardVM();
            model.allBikes = context.Bike.Where(a => !a.isArchived).ToList();
            model.latestInspections = context.Inspection.OrderByDescending(d => d.datePerformed).Take(15).ToList();
            model.latestMaintenances = context.MaintenanceEvent.Where(a => a.isArchived).OrderByDescending(d => d.timeAdded).Take(15).ToList();
            return View(model);
        }

        public ActionResult bikeDetails(int bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.Maint.bikeDetailsVM();
            model.bikeId = bikeId;
            var bike = context.Bike.Find(bikeId);
            model.bikeName = bike.bikeName;
            model.bikeNumber = bike.bikeNumber;
            model.inspections = context.Inspection.Where(b => b.bike.bikeId == bikeId).OrderByDescending(d => d.datePerformed).Take(15).ToList();
            model.maints = context.MaintenanceEvent.Where(b => b.bikeAffected.bikeId == bikeId).OrderByDescending(d => d.timeAdded).Take(15).ToList();
            model.totalInspections = context.Inspection.Where(b => b.bike.bikeId == bikeId).Count();
            model.totalCheckouts = bike.checkOuts.Count();

            return View(model);
        }

        public ActionResult maintenanceDetails(int maintId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.MaintenanceEvent.Find(maintId));
        }

        public ActionResult newMaintenance(int bikeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new MaintenanceEvent();
            model.bikeAffected = context.Bike.Find(bikeId);
            ViewBag.query = context.WorkShop.Where(a => !a.isArchived).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newMaintenance([Bind] MaintenanceEvent maint)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            maint.staffPerson = context.BikeUser.Where(n => n.userName == User.Identity.Name).First();
            maint.bikeAffected = context.Bike.Find(maint.bikeAffected.bikeId);
            maint.workshop = context.WorkShop.Find(maint.workshop.workshopId);
            maint.timeAdded = DateTime.Now;
            maint.timeResolved = new DateTime(2000, 01, 01);
            context.MaintenanceEvent.Add(maint);
            context.SaveChanges();
            return RedirectToAction("bikeDetails", new { bikeId = maint.bikeAffected.bikeId });
        }

        public ActionResult newInspection(int bikeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new Inspection();
            model.bike = context.Bike.Find(bikeId);
            ViewBag.query = context.WorkShop.Where(a => !a.isArchived).ToList();
            var vModel = new ViewModels.specWithMaint();
            vModel.spec = model;
            vModel.maint = new MaintenanceEvent();
            return View(vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newInspection([Bind] ViewModels.specWithMaint inspection)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }

            inspection.spec.inspector = context.BikeUser.Where(u => u.userName == User.Identity.Name).First();
            if (!String.IsNullOrWhiteSpace(inspection.maint.title))
            {
                inspection.spec.associatedMaintenance.Add(inspection.maint);
                inspection.maint.staffPerson = context.BikeUser.Where(u => u.userName == User.Identity.Name).First();
                context.MaintenanceEvent.Add(inspection.maint);
                context.SaveChanges();
                return RedirectToAction("maintenanceDetails", "Mechanic", new { inspection.maint.MaintenanceEventId });
            }
            inspection.spec.datePerformed = DateTime.Now;
            inspection.spec.placeInspected = context.WorkShop.Find(inspection.spec.placeInspected.workshopId);
            inspection.spec.bike = context.Bike.Find(inspection.spec.bike.bikeId);
            context.Inspection.Add(inspection.spec);
            if (inspection.spec.isPassed) { inspection.spec.bike.lastPassedInspection = DateTime.Now; }
            context.SaveChanges();
            return RedirectToAction("bikeDetails", "Mechanic", new { bikeId = inspection.spec.bike.bikeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult closeMaintenance(int maintId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var maint = context.MaintenanceEvent.Include(b => b.bikeAffected).Include(w => w.workshop).Include(u => u.staffPerson).Where(m => m.MaintenanceEventId == maintId).First();
            maint.timeResolved = DateTime.Now;
            maint.resolved = true;
            context.SaveChanges();
            return RedirectToAction("maintenanceDetails", new { maintId = maintId });
        }

        public ActionResult bikeInspections(int bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.PaginatedViewModel<Inspection>();
            model.modelList = context.Inspection.Where(b => b.bike.bikeId == bikeId).OrderByDescending(d => d.datePerformed).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            ViewBag.bike = context.Bike.Find(bikeId);
            return View(model);
        }

        public ActionResult bikeMaintenance(int bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = context.MaintenanceEvent.Where(b => b.bikeAffected.bikeId == bikeId).OrderByDescending(d => d.timeAdded).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            ViewBag.bike = context.Bike.Find(bikeId);
            return View(model);
        }

        public ActionResult bikeListing(int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.PaginatedViewModel<ViewModels.bikeCard>();
            foreach (var bike in context.Bike.Include(r => r.bikeRack).Where(a => !a.isArchived).OrderByDescending(i => i.bikeId).Skip((page - 1) * pageSize).Take(pageSize).ToList())
            {
                var card = new ViewModels.bikeCard();
                card.bikeId = bike.bikeId;
                card.bikeName = bike.bikeName;
                card.bikeNumber = bike.bikeNumber;
                card.rackId = bike.bikeRack.bikeRackId;
                card.rackName = bike.bikeRack.name;
                card.totalInspections = context.Inspection.Where(b => b.bike.bikeId == bike.bikeId).Count();
                card.totalMaintenance = context.MaintenanceEvent.Where(b => b.bikeAffected.bikeId == bike.bikeId).Count();
                card.status = ViewModels.cardStatus.defaults;
                if (card.totalInspections > 0)
                {
                    Inspection last = context.Inspection.Where(b => b.bike.bikeId == bike.bikeId).OrderByDescending(d => d.datePerformed).First();
                    card.dateLastInspected = last.datePerformed.ToShortDateString();
                    if (!last.isPassed)
                    {
                        card.status = ViewModels.cardStatus.danger;
                    }
                }
                else
                {
                    card.dateLastInspected = "never";
                }
                if (card.totalMaintenance > 0)
                {
                    card.dateLastMaintenance = context.MaintenanceEvent.Where(b => b.bikeAffected.bikeId == bike.bikeId).OrderByDescending(d => d.timeAdded).First().timeAdded.ToShortDateString();
                }
                else
                {
                    card.dateLastInspected = "never";
                }
                if (bike.lastPassedInspection.AddDays(context.settings.First().DaysBetweenInspections) < DateTime.Now)
                {
                    card.status = ViewModels.cardStatus.danger;
                }
                model.modelList.Add(card);
            }
            model.modelList = model.modelList.OrderBy(b => b.bikeNumber).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            return View(model);
        }

        public ActionResult inspectionDetails(int specId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.Inspection.Find(specId));
        }

        public ActionResult myActivity(int activityPage = 1, int hourPage = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new ViewModels.Maint.userActivityVM();
            model.cards = new List<ViewModels.ActivityCard>();
            int userId = context.BikeUser.Where(u => u.userName == User.Identity.Name).First().bikeUserId;
            model.activtyPage = new ViewModels.PageInfo(context.tracer.Where(u => u.user.userName == User.Identity.Name).Count(), activityPage, (pageSize - 1) * pageSize);
            foreach (var item in context.tracer.Where(u => u.user.userName == User.Identity.Name).OrderByDescending(d => d.time).Skip((activityPage - 1) * pageSize).Take(pageSize))
            {
                var building = new ViewModels.ActivityCard { date = item.time, userName = User.Identity.Name, userId = userId };
                if (item.inspection != null)
                {
                    building.title = "Inspection: " + item.inspection.comment.ToString();
                    if (item.inspection.isPassed) { building.status = ViewModels.cardStatus.success; } else { building.status = ViewModels.cardStatus.danger; }
                    building.type = ViewModels.activityType.inspection;
                }
                if (item.maint != null)
                {
                    building.title = "Maintenance: " + item.maint.title;
                    if (item.maint.resolved) { building.status = ViewModels.cardStatus.success; } else { building.status = ViewModels.cardStatus.danger; }
                    building.type = ViewModels.activityType.maintenance;
                }
                if (item.update != null)
                {
                    building.title = "Comment on Maintenance: " + item.update.title;
                    building.status = ViewModels.cardStatus.defaults;
                    building.type = ViewModels.activityType.comment;
                }
                model.cards.Add(building);
            }

            model.hours = context.workHours.Where(u => u.user.userName == User.Identity.Name).OrderByDescending(d => d.timeEnd).Skip((hourPage - 1) * pageSize).Take(pageSize).ToList();
            model.hoursPage = new ViewModels.PageInfo(context.workHours.Where(u => u.user.userName == User.Identity.Name).Count(), pageSize, hourPage);
            return View(model);
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
            MaintenanceUpdate comment = new MaintenanceUpdate
            {
                associatedEvent = context.MaintenanceEvent.Find(maintenanceId),
                body = commentBody,
                isCommentOnBike = false,
                postedBy = context.BikeUser.Where(u => u.userName == User.Identity.Name).First(),
                timePosted = DateTime.Now,
                title = commentTitle
            };
            context.MaintenanceEvent.Find(maintenanceId).updates.Add(comment);
            context.SaveChanges();
            return RedirectToAction("MaintenanceDetails", "Mechanic", new { maintId = maintenanceId });
        }

        public ActionResult newHour()
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new WorkHour
            {
                user = context.BikeUser.Where(u => u.userName == User.Identity.Name).First(),
                timeEnd = DateTime.Now.AddHours(1),
                timeStart = DateTime.Now
            };
            ViewBag.query = context.WorkShop.Where(a => !a.isArchived);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newHour([Bind] WorkHour hour, int userId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            hour.user = context.BikeUser.Find(userId);
            hour.maint = null;
            hour.rack = null;
            hour.shop = context.WorkShop.Find(hour.shop.workshopId);
            context.workHours.Add(hour);
            context.SaveChanges();
            return RedirectToAction("myActivity", "Mechanic");
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
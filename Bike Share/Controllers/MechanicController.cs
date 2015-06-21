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
            return View(context.Bike.Where(a => !a.isArchived).OrderByDescending(i => i.bikeNumber).ToList());
        }

        public ActionResult bikeDetails(int bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.Maint.bikeDetailsVM();
            model.bikeId = bikeId;
            var bike = context.Bike.Find(bikeId);
            model.bikeName = bike.bikeName;
            model.bikeNumber = bike.bikeNumber;
            model.inspections = context.Inspection.Where(b => b.bikeId == bikeId).OrderByDescending(d => d.datePerformed).Take(15).ToList();
            model.maints = context.MaintenanceEvent.Where(b => b.bikeId == bikeId).OrderByDescending(d => d.timeAdded).Take(15).ToList();
            model.totalInspections = context.Inspection.Where(b => b.bikeId == bikeId).Count();
            model.totalCheckouts = context.CheckOut.Where(b => b.bike == bikeId).Count();

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
            model.bikeId = bikeId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newMaintenance([Bind] MaintenanceEvent maint)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            maint.submittedById = context.BikeUser.Where(n => n.userName == User.Identity.Name).First().bikeUserId;
            maint.timeAdded = DateTime.Now;
            context.MaintenanceEvent.Add(maint);
            context.SaveChanges();
            return RedirectToAction("bikeDetails", new { bikeId = maint.bikeId });
        }

        public ActionResult newInspection(int bikeId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new Inspection();
            model.bikeId = bikeId;
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

            inspection.spec.inspectorId = context.BikeUser.Where(u => u.userName == User.Identity.Name).First().bikeUserId;
            if (!String.IsNullOrWhiteSpace(inspection.maint.title))
            {
                inspection.maint.submittedById = context.BikeUser.Where(u => u.userName == User.Identity.Name).First().bikeUserId;
                context.MaintenanceEvent.Add(inspection.maint);
                context.SaveChanges();
                return RedirectToAction("maintenanceDetails", "Mechanic", new { inspection.maint.MaintenanceEventId });
            }
            inspection.spec.datePerformed = DateTime.Now;
            context.Inspection.Add(inspection.spec);
            context.Bike.Find(inspection.spec.bikeId).onInspectionHold = !inspection.spec.isPassed; 
            context.SaveChanges();
            return RedirectToAction("bikeDetails", "Mechanic", new { bikeId = inspection.spec.bikeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult closeMaintenance(int maintId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var maint = context.MaintenanceEvent.Where(m => m.MaintenanceEventId == maintId).First();
            maint.timeResolved = DateTime.Now;
            maint.maintainedById = context.BikeUser.Where(u => u.userName == User.Identity.Name).First().bikeUserId;
            context.SaveChanges();
            return RedirectToAction("maintenanceDetails", new { maintId = maintId });
        }

        public ActionResult bikeInspections(int bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.PaginatedViewModel<Inspection>();
            model.modelList = context.Inspection.Where(b => b.bikeId == bikeId).OrderByDescending(d => d.datePerformed).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            ViewBag.bike = context.Bike.Find(bikeId);
            return View(model);
        }

        public ActionResult bikeMaintenance(int bikeId, int page = 1)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new BikeShare.ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = context.MaintenanceEvent.Where(b => b.bikeId == bikeId).OrderByDescending(d => d.timeAdded).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            ViewBag.bike = context.Bike.Find(bikeId);
            return View(model);
        }

        public ActionResult inspectionDetails(int specId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            return View(context.Inspection.Find(specId));
        }

      
        public ActionResult newComment(int maintId)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            var model = new MaintenanceUpdate();
            model.associatedEventId = maintId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newComment(int maintenanceId, string commentTitle, string commentBody)
        {
            if (!authorize()) { return RedirectToAction("authError", "Error"); }
            MaintenanceUpdate comment = new MaintenanceUpdate
            {
                associatedEventId = maintenanceId,
                body = commentBody,
                isCommentOnBike = false,
                postedById = context.BikeUser.Where(u => u.userName == User.Identity.Name).First().bikeUserId,
                timePosted = DateTime.Now,
                title = commentTitle
            };
            context.MaintenanceUpdate.Add(comment);
            context.SaveChanges();
            return RedirectToAction("MaintenanceDetails", "Mechanic", new { maintId = maintenanceId });
        }


        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Interfaces;
using BikeShare.Models;

namespace BikeShare.Controllers
{
    [Authorize]
    public class MechanicController : Controller
    {
        private IMaintenanceRepository repo;
        private IUserRepository userRepo;
        private IExploreRepository exploreRepo;
        private ISettingRepository setRepo;
        private int pageSize = 25;
        public MechanicController(IMaintenanceRepository repo, IUserRepository uRepo, IExploreRepository eRepo, ISettingRepository sRepo)
        {
            this.repo = repo;
            this.userRepo = uRepo;
            this.exploreRepo = eRepo;
            this.setRepo = sRepo;
        }
        /// <summary>
        /// Displays the mechanic's home page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new BikeShare.ViewModels.Maint.dashboardVM();
            model.allBikes = repo.getSomeBikes(0, 100);
            model.latestInspections = repo.getSomeInspections(0, 10, true, true);
            model.latestMaintenances = repo.getSomeMaintenance(0, 10, false);
            return View(model);
        }

        public ActionResult bikeDetails(int bikeId, int page = 1)
        {
            var model = new BikeShare.ViewModels.Maint.bikeDetailsVM();
            model.bikeId = bikeId;
            var bike = repo.getBikeById(bikeId);
            model.bikeName = bike.bikeName;
            model.bikeNumber = bike.bikeNumber;
            model.inspections = repo.getInspectionsForBike(bikeId, 0, 10, true, true).ToList();
            model.maints = repo.getMaintenanceForBike(bikeId, 0, 10, false).ToList();
            model.totalInspections = repo.totalInspectionsForBike(bikeId);
            model.totalCheckouts = bike.checkOuts.Count();

            return View(model);
        }

        public ActionResult maintenanceDetails(int maintId)
        {
            return View(repo.getMaintenanceById(maintId));
        }

        public ActionResult newMaintenance(int bikeId)
        {
            var model = new MaintenanceEvent();
            model.bikeAffected = repo.getBikeById(bikeId);
            ViewBag.query = repo.getAllWorkshops();
            return View(model);
        }

        [HttpPost]
        public ActionResult newMaintenance([Bind] MaintenanceEvent maint)
        {
            repo.newMaintenance(maint.bikeAffected.bikeId, maint.title, maint.details, User.Identity.Name, maint.workshop.workshopId, maint.disableBike);
            return RedirectToAction("bikeDetails", new { bikeId = maint.bikeAffected.bikeId});
        }

        public ActionResult newInspection(int bikeId)
        {
            var model = new Inspection();
            model.bike = repo.getBikeById(bikeId);
            ViewBag.query = repo.getAllWorkshops();
            var vModel = new ViewModels.specWithMaint();
            vModel.spec = model;
            vModel.maint = new MaintenanceEvent();
            return View(vModel);
        }

        [HttpPost]
        public ActionResult newInspection([Bind] ViewModels.specWithMaint inspection)
        {
            int specId = repo.newInspection(inspection.spec.bike.bikeId, User.Identity.Name, inspection.spec.placeInspected.workshopId, inspection.spec.isPassed, inspection.spec.comment);
            if (!String.IsNullOrWhiteSpace(inspection.maint.title))
            {
                int maintId = repo.newMaintenance(inspection.spec.bike.bikeId, inspection.maint.title, inspection.maint.details, User.Identity.Name, inspection.spec.placeInspected.workshopId, inspection.maint.disableBike);
                repo.addMaintToInspection(maintId, specId);
                return RedirectToAction("maintenanceDetails", "Mechanic", new { maintId = maintId });
            }
            return RedirectToAction("bikeDetails", "Mechanic", new { bikeId = inspection.spec.bike.bikeId});
        }

        [HttpPost]
        public ActionResult closeMaintenance(int maintId)
        {
            repo.closeMaint(maintId);
            return RedirectToAction("maintenanceDetails", new { maintId = maintId});
        }

        public ActionResult bikeInspections(int bikeId, int page = 1)
        {
            var model = new BikeShare.ViewModels.PaginatedViewModel<Inspection>();
            model.modelList = repo.getInspectionsForBike(bikeId, (page - 1) * pageSize, pageSize, true, true).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            ViewBag.bike = repo.getBikeById(bikeId);
            return View(model);
        }
        
        public ActionResult bikeMaintenance(int bikeId, int page = 1)
        {
            var model = new BikeShare.ViewModels.PaginatedViewModel<MaintenanceEvent>();
            model.modelList = repo.getMaintenanceForBike(bikeId, (page - 1) * pageSize, pageSize, false).ToList();
            model.pagingInfo = new ViewModels.PageInfo(model.modelList.Count(), pageSize, page);
            ViewBag.bike = repo.getBikeById(bikeId);
            return View(model);
        }

        public ActionResult bikeListing(int page = 1)
        {
            var model = new ViewModels.PaginatedViewModel<ViewModels.bikeCard>();
            foreach (var bike in repo.getSomeBikes((page - 1) * pageSize, pageSize))
            {
                var card = new ViewModels.bikeCard();
                card.bikeId = bike.bikeId;
                card.bikeName = bike.bikeName;
                card.bikeNumber = bike.bikeNumber;
                card.rackId = bike.bikeRack.bikeRackId;
                card.rackName = bike.bikeRack.name;
                card.totalInspections = repo.totalInspectionsForBike(bike.bikeId);
                card.totalMaintenance = repo.totalMaintForBike(bike.bikeId);
                card.status = ViewModels.cardStatus.defaults;
                if (card.totalInspections > 0)
                {
                    card.dateLastInspected = repo.getInspectionsForBike(bike.bikeId, 0, 1, true, true).First().datePerformed.ToShortDateString();
                    if (!repo.getInspectionsForBike(bike.bikeId, 0, 1, true, true).First().isPassed)
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
                    card.dateLastMaintenance = repo.getMaintenanceForBike(bike.bikeId, 0, 1, false).First().timeAdded.ToShortDateString();
                }
                else
                {
                    card.dateLastInspected = "never";
                }
                if (bike.lastPassedInspection.AddDays(setRepo.getDaysBetweenInspections()) < DateTime.Now)
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
            return View(repo.getInspectionById(specId));
        }

        public ActionResult myActivity(int activityPage = 1, int hourPage = 1)
        {
            var model = new ViewModels.Maint.userActivityVM();
            model.cards = new List<ViewModels.ActivityCard>();
            int userId = userRepo.getUserByName(User.Identity.Name).bikeUserId;
            model.activtyPage = new ViewModels.PageInfo(exploreRepo.countEventsForUser(userId), pageSize, (activityPage - 1) * pageSize);
            foreach (var item in exploreRepo.getSomeEvents(userId, (activityPage - 1) * pageSize, pageSize))
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

            model.hours = repo.getHoursForUser(userId, (hourPage - 1) * pageSize, pageSize).ToList();
            model.hoursPage = new ViewModels.PageInfo(repo.getAllHoursForUser(userId).Count(), pageSize, hourPage);
            return View(model);
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
            repo.commentOnMaint(maintenanceId, commentTitle, commentBody, User.Identity.Name);
            return RedirectToAction("MaintenanceDetails", "Mechanic", new { maintId = maintenanceId});
        }

        public ActionResult newHour()
        {
            var model = new WorkHour();
            model.user = userRepo.getUserByName(User.Identity.Name);
            model.timeStart = DateTime.Now;
            model.timeEnd = DateTime.Now.AddHours(1);
            ViewBag.query = repo.getAllWorkshops();
            return View(model);
        }

        [HttpPost]
        public ActionResult newHour([Bind] WorkHour hour, int userId)
        {
            repo.recordHours(userId, hour.timeStart, hour.timeEnd, hour.comment, null, null, null);
            return RedirectToAction("myActivity", "Mechanic");
        }
    }
}
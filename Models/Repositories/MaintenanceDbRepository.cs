using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;
using BikeShare.Interfaces;
using System.Data.Entity;

namespace BikeShare.Repositories
{
    public class MaintenanceDbRepository : IMaintenanceRepository
    {

        public int newMaintenance(int bikeId, string title, string details, string userName, int workshopId, bool disableBike)
        {
            using (var db = new BikesContext())
            {
                var maint = new MaintenanceEvent();
                maint.bikeAffected = db.Bike.Find(bikeId);
                maint.details = details;
                maint.disableBike = disableBike;
                maint.isArchived = false;
                maint.resolved = false;
                maint.staffPerson = db.BikeUser.Where(n => n.userName == userName).First();
                maint.timeAdded = DateTime.Now;
                maint.timeResolved = new DateTime(2000, 1, 1);
                maint.title = title;
                maint.workshop = db.WorkShop.Find(workshopId);
                db.MaintenanceEvent.Add(maint);
                var trace = new Tracer();
                trace.user = maint.staffPerson;
                trace.maint = maint;
                trace.time = DateTime.Now;
                trace.comment = "User Submitted Maintenance";
                db.tracer.Add(trace);
                db.SaveChanges();
                return maint.MaintenanceEventId;
            }
        }

        public Bike getBikeById(int bikeId)
        {
            using (var db = new BikesContext())
            {
                return (Bike)db.Bike.Include(b => b.bikeRack).Include(c => c.checkOuts).Where(i => i.bikeId == bikeId).First();
            }
        }

        public MaintenanceEvent getMaintenanceById(int maintId)
        {
            using (var db =  new BikesContext())
            {
                return (MaintenanceEvent) db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson).Include(u => u.updates)
                    .Include(w => w.workshop).Where(i => i.MaintenanceEventId == maintId).First();
            }
        }

        public IEnumerable<MaintenanceEvent> getSomeMaintenance(int skip, int count, bool unResolvedOnly)
        {
            using (var db = new BikesContext())
            {
                if (unResolvedOnly)
                {
                    return db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson).Include(u => u.updates)
                        .Include(w => w.workshop).Where(r => !r.resolved).OrderByDescending(d => d.timeAdded).Skip(skip).Take(count).ToList();
                }
                else
                {
                    return db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson).Include(u => u.updates)
                        .Include(w => w.workshop).OrderByDescending(d => d.timeAdded).Skip(skip).Take(count).ToList();
 
                }

            }
        }

        public void commentOnMaint(int maintId, string title, string body, string userName)
        {
            using (var db = new BikesContext())
            {
                var update = new MaintenanceUpdate();
                update.associatedEvent = db.MaintenanceEvent.Find(maintId);
                update.body = body;
                update.postedBy = db.BikeUser.Where(u => u.userName == userName).First();
                update.timePosted = DateTime.Now;
                update.title = title;
                db.MaintenanceUpdate.Add(update);
                var trace = new Tracer();
                trace.user = update.postedBy;
                trace.update = update;
                trace.time = DateTime.Now;
                trace.maint = update.associatedEvent;
                trace.comment = "User Commented on Maintenance";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void closeMaint(int maintId)
        {
            using (var db = new BikesContext())
            {
                var maint = db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson).Include(u => u.updates)
                        .Include(w => w.workshop).Where(i => i.MaintenanceEventId == maintId).First();
                maint.timeResolved = DateTime.Now;
                maint.resolved = true;
                var trace = new Tracer();
                trace.user = maint.staffPerson;
                trace.maint = maint;
                trace.time = DateTime.Now;
                trace.comment = "User Closed Maintenance";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void archiveMaint(int maintId)
        {
            using (var db = new BikesContext())
            {
                var maint = db.MaintenanceEvent.Find(maintId);
                maint.isArchived = true;
                var trace = new Tracer();
                trace.user = maint.staffPerson;
                trace.maint = maint;
                trace.time = DateTime.Now;
                trace.comment = "User Archived Maintenance";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public int newInspection(int bikeId, string userName, int workshopId, bool isPassed, string comment)
        {
            using (var db = new BikesContext())
            {
                var inspection = new Inspection();
                inspection.bike = db.Bike.Find(bikeId);
                if (isPassed)
                {
                    var bike = db.Bike.Find(bikeId);
                    bike.lastPassedInspection = DateTime.Now;
                }
                inspection.comment = comment;
                inspection.datePerformed = DateTime.Now;
                inspection.inspector = db.BikeUser.Where(n => n.userName == userName).First();
                inspection.isPassed = isPassed;
                inspection.placeInspected = db.WorkShop.Find(workshopId);
                db.Inspection.Add(inspection);
                var trace = new Tracer();
                trace.user = inspection.inspector;
                trace.inspection = inspection;
                trace.time = DateTime.Now;
                trace.comment = "User Created new Inspection";
                db.tracer.Add(trace);
                db.SaveChanges();
                return inspection.inspectionId;
            }
        }

        public void addMaintToInspection(int maintId, int inspectionId)
        {
            using (var db = new BikesContext())
            {
                db.Inspection.Find(inspectionId).associatedMaintenance.Add(db.MaintenanceEvent.Find(maintId));
                var trace = new Tracer();
                trace.inspection = db.Inspection.Find(inspectionId);
                trace.time = DateTime.Now;
                trace.comment = "User Associated Maintenance with an inspection";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }


        public IEnumerable<Inspection> getSomeInspections(int skip, int count, bool includePassed, bool includeFailed)
        {
            using (var db = new BikesContext())
            {
                var query = db.Inspection.Include(u => u.inspector).Include(p => p.placeInspected).Include(b => b.bike).Include(m => m.associatedMaintenance).OrderByDescending(d => d.datePerformed);
                if (!includePassed)
                {
                    query = query.Where(p => !p.isPassed).OrderByDescending(d => d.datePerformed);
                }
                if (!includeFailed)
                {
                    query = query.Where(p => p.isPassed).OrderByDescending(d => d.datePerformed);
                }
                return query.Skip(skip).Take(count).ToList();
            }
        }

        public IEnumerable<Inspection> getInspectionsForBike(int bikeId, int skip, int count, bool includePassed, bool includeFailed)
        {
            using (var db = new BikesContext())
            {
                var query = db.Inspection.Include(u => u.inspector).Include(p => p.placeInspected).Include(b => b.bike).Include(m => m.associatedMaintenance).Where(b => b.bike.bikeId == bikeId).OrderByDescending(d => d.datePerformed);
                if (!includePassed)
                {
                    query = query.Where(p => !p.isPassed).OrderByDescending(d => d.datePerformed);
                }
                if (!includeFailed)
                {
                    query = query.Where(p => p.isPassed).OrderByDescending(d => d.datePerformed);
                }
                return query.Skip(skip).Take(count).ToList();
            }
        }

        public IEnumerable<MaintenanceEvent> getMaintenanceForBike(int bikeId, int skip, int count, bool unResolvedOnly)
        {
            using (var db = new BikesContext())
            {
                if (unResolvedOnly)
                {
                    return db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson).Include(u => u.updates)
                        .Include(w => w.workshop).Where(i => i.bikeAffected.bikeId == bikeId).Where(r => !r.resolved).OrderByDescending(d => d.timeAdded).Skip(skip).Take(count).ToList();
                }
                else
                {
                    return db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson).Include(u => u.updates)
                        .Include(w => w.workshop).Where(i => i.bikeAffected.bikeId == bikeId).OrderByDescending(d => d.timeAdded).Skip(skip).Take(count).ToList();

                }

            }
        }


        public int totalInspectionsForBike(int bikeId)
        {
            using (var db = new BikesContext())
            {
                return db.Inspection.Include(b => b.bike).Where(i => i.bike.bikeId == bikeId).Count();
            }
        }

        public int totalMaintForBike(int bikeId)
        {
            using (var db = new BikesContext())
            {
                return db.MaintenanceEvent.Include(b => b.bikeAffected).Where(i => i.bikeAffected.bikeId == bikeId).Count();
            }
        }


        public IEnumerable<Bike> getSomeBikes(int skip, int count)
        {
            using (var db = new BikesContext())
            {
                return db.Bike.Include(b => b.bikeRack).Include(c => c.checkOuts).OrderByDescending(l => l.lastCheckedOut).Skip(skip).Take(count).ToList();
            }
        }


        public IEnumerable<Workshop> getAllWorkshops()
        {
            using (var db = new BikesContext())
            {
                return db.WorkShop.Include(u => u.maintenanceEvents).ToList();
            }
        }


        public Inspection getInspectionById(int specId)
        {
            using (var db = new BikesContext())
            {
                return db.Inspection.Include(u => u.inspector).Include(p => p.placeInspected).Include(b => b.bike).Include(m => m.associatedMaintenance).Where(i => i.inspectionId == specId).First();
            }
        }


        public IEnumerable<WorkHour> getAllHoursForUser(int userId)
        {
            using (var db = new BikesContext())
            {
                return db.workHours.Include(u => u.user).Include(m => m.maint).Include(b => b.bike).Include(w => w.shop).Where(u => u.user.bikeUserId == userId).OrderByDescending(d => d.timeEnd).ToList();
            }
        }

        public IEnumerable<WorkHour> getHoursForUser(int userId, int skip, int take)
        {
            using (var db = new BikesContext())
            {
                return db.workHours.Include(u => u.user).Include(m => m.maint).Include(b => b.bike).Include(w => w.shop)
                    .Where(u => u.user.bikeUserId == userId).OrderByDescending(d => d.timeEnd).Skip(skip).Take(take).ToList();
            }
        }

        public void recordHours(int userId, DateTime start, DateTime end, string comment, int? bikeId, int? maintenanceId, int? workshopId)
        {
            using (var db = new BikesContext())
            {
                var hour = new WorkHour();
                hour.user = db.BikeUser.Find(userId);
                hour.timeStart = start;
                hour.timeEnd = end;
                hour.comment = comment;
                if (bikeId != null)
                {
                    hour.bike = db.Bike.Find(bikeId);
                }
                if (maintenanceId != null)
                {
                    hour.maint = db.MaintenanceEvent.Find(maintenanceId);
                }
                if (workshopId != null)
                {
                    hour.shop = db.WorkShop.Find(workshopId);
                }
                db.workHours.Add(hour);
                db.SaveChanges();
            }
        }


        public int countHoursForUser(int userId)
        {
            using (var db = new BikesContext())
            {
                return db.workHours.Include(u => u.user).Where(i => i.user.bikeUserId == userId).Count();
            }
        }
    }
}

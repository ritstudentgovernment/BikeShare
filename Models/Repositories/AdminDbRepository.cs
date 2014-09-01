using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Interfaces;
using BikeShare.Models;

namespace BikeShare.Repositories
{
    public class AdminDbRepository : IAdminRepository
    {
        private BikesContext db = new BikesContext();

        public IEnumerable<Models.Bike> getAllBikes()
        {
            return db.Bike.Include(b => b.bikeRack).Include(b => b.checkOuts).Where(a => a.isArchived == false).ToList();
        }

        public IEnumerable<Models.bikeUser> getAllMaintenancePeople()
        {
            return db.BikeUser.Where(m => m.canMaintainBikes == true).Where(a => a.isArchived == false).ToList();
        }

        public IEnumerable<Models.bikeUser> getAllAppAdmins()
        {
            return db.BikeUser.Where(m => m.canAdministerSite == true).Where(a => a.isArchived == false).ToList();
        }

        public IEnumerable<Models.BikeRack> getAllBikeRacks()
        {
            return db.BikeRack.Include(b => b.bikes).Include(c => c.checkOuts).Where(a => a.isArchived == false).ToList();
        }

        public IEnumerable<Models.CheckOut> getAllCurrentCheckOuts()
        {
            return db.CheckOut.Include(b => b.bike).Include(p => p.checkOutPerson).Include(u => u.user).Include(b => b.rackCheckedIn).Where(i => i.isResolved == false).ToList();
        }

        public void archiveBike(int bikeId)
        {
            var bike = db.Bike.Include(r => r.bikeRack).Where(b => b.bikeId == bikeId).First();
            bike.isArchived = true;
            var trace = new Tracer();
            trace.bike = bike;
            trace.time = DateTime.Now;
            trace.comment = "Bike was archived";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void archiveUser(int bikeUserid)
        {
            var user = db.BikeUser.Find(bikeUserid);
            user.isArchived = true;
            var trace = new Tracer();
            trace.user = user;
            trace.time = DateTime.Now;
            trace.comment = "User was archived.";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void archiveBikeRack(int rackId)
        {
            db.BikeRack.Include(b => b.bikes).Include(c => c.checkOuts).Where(i => i.bikeRackId == rackId).First().isArchived = true;
            var trace = new Tracer();
            trace.rack = db.BikeRack.Find(rackId);
            trace.time = DateTime.Now;
            trace.comment = "User Checked out Bike";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void addBike(Models.Bike bike)
        {
            bike.lastPassedInspection = new DateTime(2000, 1, 1);
            db.Bike.Add(bike);
            var trace = new Tracer();
            trace.bike = bike;
            trace.time = DateTime.Now;
            trace.comment = "User Checked out Bike";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void addBikeRack(Models.BikeRack rack)
        {
            db.BikeRack.Add(rack);
            var trace = new Tracer();
            trace.rack = rack;
            trace.time = DateTime.Now;
            trace.comment = "User Checked out Bike";
            db.tracer.Add(trace);
            db.SaveChanges();
        }


        public IEnumerable<Bike> getSomeBikes(int count, int skip)
        {
            return getAllBikes().OrderByDescending(b => b.lastCheckedOut).Where(a => a.isArchived == false).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<Bike> getOverdueBikes(int count, int skip)
        {
            return db.Bike.Include(r => r.bikeRack).Include(c => c.checkOuts).OrderByDescending(b => b.lastCheckedOut)
                .Where(o => o.checkOuts.Any(c => c.isResolved == false)).Where(a => a.isArchived == false).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<BikeRack> getSomeBikeRacks(int count, int skip)
        {
            return getAllBikeRacks().Where(a => a.isArchived == false).OrderByDescending(i => i.bikeRackId).Skip(skip).Take(count);
        }

        public IEnumerable<bikeUser> getAllUsers(bool includeCheckOut, bool includeRiders, bool includeMechanics, bool includeAdmins)
        {
            var users = db.BikeUser.Include(m => m.maintenanceEvents)
                .Include(c => c.charges).Include(c => c.checkOuts).Where(a => a.isArchived == false);
            if (!includeCheckOut)
            {
                users = users.Where(p => p.canCheckOutBikes == false);
            }
            if (!includeRiders)
            {
                users = users.Where(p => p.canBorrowBikes == false);
            }
            if (!includeMechanics)
            {
                users = users.Where(p => p.canMaintainBikes == false);
            }
            if (!includeAdmins)
            {
                users = users.Where(p => p.canAdministerSite == false);
            }
            return users.ToList();
        }

        public IEnumerable<bikeUser> getSomeUsers(int count, int skip, bool includeCheckOut, bool includeRiders, bool includeMechanics, bool includeAdmins)
        {

            return getAllUsers(includeCheckOut, includeRiders, includeMechanics, includeAdmins).Where(a => a.isArchived == false)
                .OrderByDescending(u => u.lastRegistered).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<CheckOut> getAllCheckouts()
        {
            return db.CheckOut.Include(b => b.bike).Include(r => r.rackCheckedIn)
                    .Include(r => r.rackCheckedOut).Include(u => u.user).Include(c => c.checkOutPerson).ToList();
        }

        public IEnumerable<CheckOut> getSomeCheckouts(int count, int skip, bool excludeResolved)
        {
            if (excludeResolved)
            {
                return db.CheckOut.Include(b => b.bike).Include(r => r.rackCheckedIn)
                    .Include(r => r.rackCheckedOut).Include(u => u.user).Include(c => c.checkOutPerson)
                    .Where(i => i.isResolved == false).OrderByDescending(c => c.timeOut).Skip(skip).Take(count).ToList();
            }
            else
            {
                return db.CheckOut.Include(b => b.bike).Include(r => r.rackCheckedIn)
                    .Include(r => r.rackCheckedOut).Include(u => u.user).Include(c => c.checkOutPerson)
                    .OrderByDescending(c => c.timeOut).Skip(skip).Take(count).ToList();
            }
        }

        public IEnumerable<Workshop> getAllWorkshops()
        {
            return db.WorkShop.Include(m => m.maintenanceEvents).Where(a => a.isArchived == false).ToList();
        }

        public IEnumerable<Workshop> getSomeWorkshops(int count, int skip)
        {
            return db.WorkShop.Include(m => m.maintenanceEvents).Where(a => a.isArchived == false)
                .OrderByDescending(w => w.workshopId).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<Inspection> getSomeInspections(int count, int skip)
        {
            return db.Inspection.Include(b => b.bike)
                .Include(w => w.placeInspected).Include(u => u.inspector)
                .OrderByDescending(i => i.datePerformed).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<MaintenanceEvent> getAllMaintenance()
        {
            return db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson)
                .Include(w => w.workshop).Where(a => a.isArchived == false).ToList();
        }

        public IEnumerable<MaintenanceEvent> getSomeMaintenance(int count, int skip)
        {
            return db.MaintenanceEvent.Include(b => b.bikeAffected).Include(w => w.workshop)
                .Where(a => a.isArchived == false).Include(u => u.staffPerson)
                .OrderByDescending(m => m.timeAdded).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<MaintenanceEvent> getMaintenanceForBike(int count, int skip, int bikeId)
        {
            return db.MaintenanceEvent.Include(b => b.bikeAffected).Where(i => i.bikeAffected.bikeId == bikeId)
                .Where(a => a.isArchived == false).OrderByDescending(m => m.timeAdded).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<MaintenanceEvent> getMaintenanceForUser(int count, int skip, int userId)
        {
            return db.MaintenanceEvent.Include(u => u.staffPerson).Where(u => u.staffPerson.bikeUserId == userId)
                .Where(a => a.isArchived == false).OrderByDescending(m => m.timeAdded).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<MaintenanceEvent> getMaintenanceForWorkshop(int count, int skip, int workshopId)
        {
            return db.MaintenanceEvent.Include(w => w.workshop).Include(u => u.staffPerson)
                .Where(a => a.isArchived == false).Include(b => b.bikeAffected).OrderByDescending(m => m.timeAdded).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<MaintenanceUpdate> getUpdatesForMaintenance(int count, int skip, int maintId)
        {
            return db.MaintenanceUpdate.Include(m => m.associatedEvent)
                .Where(i => i.associatedEvent.MaintenanceEventId == maintId).OrderByDescending(m => m.timePosted).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<bikeUser> getUsersWithBalances(int count, int skip)
        {
            return db.BikeUser.Include(c => c.checkOuts).Include(c => c.charges).Where(a => a.isArchived == false)
                .Where(c => c.charges.Any(o => o.isResolved == false)).OrderByDescending(m => m.lastRegistered).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<bikeUser> getArchivedUsers(int count, int skip)
        {
            return db.BikeUser.Include(c => c.charges).Include(m => m.maintenanceEvents)
                .Include(p => p.checkOuts).Where(a => a.isArchived).OrderByDescending(m => m.lastRegistered).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<BikeRack> getArchivedRacks(int count, int skip)
        {
            return db.BikeRack.Include(c => c.checkOuts).Where(a => a.isArchived)
                .OrderByDescending(r => r.bikeRackId).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<Bike> getArchivedBikes(int count, int skip)
        {
            return db.Bike.Include(c => c.checkOuts).Include(r => r.bikeRack)
                .Where(a => a.isArchived).OrderByDescending(b => b.lastCheckedOut).Skip(skip).Take(count).ToList();
        }

        public void archiveMaintenanceEvent(int maintenanceId)
        {
            var maintenance = db.MaintenanceEvent.Find(maintenanceId);
            maintenance.isArchived = true;
            var trace = new Tracer();
            trace.maint = maintenance;
            trace.time = DateTime.Now;
            trace.comment = "Maintenance Archived.";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void updateUser(bikeUser user)
        {
            var oldUser = db.BikeUser.Find(user.bikeUserId);
            oldUser.email = user.email;
            oldUser.isArchived = user.isArchived;
            oldUser.phoneNumber = user.phoneNumber;
            oldUser.canAdministerSite = user.canAdministerSite;
            oldUser.canBorrowBikes = user.canBorrowBikes;
            oldUser.canCheckOutBikes = user.canCheckOutBikes;
            oldUser.canMaintainBikes = user.canMaintainBikes;
            var trace = new Tracer();
            trace.user = oldUser;
            trace.time = DateTime.Now;
            trace.comment = "User Updated.";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void promoteUser(int userId, bool canCheckOut, bool canBorrow, bool canMaintain, bool canManageApp)
        {
            var user = db.BikeUser.Where(u => u.bikeUserId == userId).First();
            user.canAdministerSite = canManageApp;
            user.canBorrowBikes = canBorrow;
            user.canCheckOutBikes = canCheckOut;
            user.canMaintainBikes = canMaintain;
            var trace = new Tracer();
            trace.user = user;
            trace.time = DateTime.Now;
            trace.comment = "User Promoted.";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void updateBike(Bike bike)
        {
            var oldBike = db.Bike.Include(l => l.bikeRack).Where(i => i.bikeId == bike.bikeId).First();
            oldBike.bikeName = bike.bikeName;
            oldBike.bikeNumber = bike.bikeNumber;
            oldBike.isArchived = bike.isArchived;
            var trace = new Tracer();
            trace.bike = oldBike;
            trace.time = DateTime.Now;
            trace.comment = "Bike updated";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void updateBikeRack(BikeRack rack)
        {
            var oldRack = db.BikeRack.Find(rack.bikeRackId);
            oldRack.description = rack.description;
            oldRack.GPSCoordX = rack.GPSCoordX;
            oldRack.GPSCoordY = rack.GPSCoordY;
            oldRack.isArchived = rack.isArchived;
            oldRack.name = rack.name;
            var trace = new Tracer();
            trace.rack = oldRack;
            trace.time = DateTime.Now;
            trace.comment = "Bike Rack updated.";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void updateWorkshop(Workshop shop)
        {
            var oldWorkshop = db.WorkShop.Find(shop.workshopId);
            oldWorkshop.GPSCoordX = shop.GPSCoordX;
            oldWorkshop.GPSCoordY = shop.GPSCoordY;
            oldWorkshop.isArchived = shop.isArchived;
            oldWorkshop.name = shop.name;
            var trace = new Tracer();
            trace.shop = shop;
            trace.time = DateTime.Now;
            trace.comment = "Workshop updated";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void updateCharge(Charge charge)
        {
            var oldCharge = db.Charge.Find(charge.chargeId);
            oldCharge.dateResolved = charge.dateResolved;
            oldCharge.description = charge.description;
            oldCharge.isResolved = charge.isResolved;
            oldCharge.notificationsCounter = charge.notificationsCounter;
            oldCharge.title = charge.title;
            var trace = new Tracer();
            trace.charge = oldCharge;
            trace.time = DateTime.Now;
            trace.user = oldCharge.user;
            trace.comment = "charge updated";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public Bike getBikeById(int id)
        {
            return db.Bike.Include(c => c.checkOuts).Include(r => r.bikeRack).Where(i => i.bikeId == id).First();
        }

        public BikeRack getRackById(int id)
        {
            return db.BikeRack.Include(b => b.bikes).Include(c => c.checkOuts)
                .Where(i => i.bikeRackId == id).First();
        }

        public bikeUser getUserById(int id)
        {
            var user = db.BikeUser.Find(id);
            user.maintenanceEvents = db.MaintenanceEvent.Where(u => u.staffPerson.bikeUserId == id).ToList();
            user.checkOuts = db.CheckOut.Where(u => u.user.bikeUserId == id).ToList();
            //user.charges = db.Charge.Include(u => u.user).Where(u => u.user.bikeUserId == id).ToList();
            return user;

            //return db.BikeUser.Include(i => i.maintenanceEvents)
            //.Include(c => c.checkOuts).Include(c => c.charges).Include(z => z.bike)
            //.Where(i => i.bikeUserId == id).First();
            
            
        }

        public Workshop getWorkshopById(int id)
        {
            return db.WorkShop.Include(e => e.maintenanceEvents).Where(i => i.workshopId == id).First();
        }

        public MaintenanceEvent getMaintenanceById(int id)
        {
            return db.MaintenanceEvent.Include(b => b.bikeAffected).Include(u => u.staffPerson)
                .Include(w => w.workshop).Include(u => u.updates).Where(i => i.MaintenanceEventId == id).First();
        }

        public MaintenanceUpdate getMaintenanceUpdateById(int id)
        {
            return db.MaintenanceUpdate.Include(u => u.postedBy).Include(e => e.associatedEvent)
                .Where(i => i.MaintenanceUpdateId == id).First();
        }

        public Charge getChargeById(int id)
        {
            return db.Charge.Include(u => u.user).Where(i => i.chargeId == id).First();
        }

        public Inspection getInspectionById(int id)
        {
            return db.Inspection.Include(b => b.bike).Include(w => w.placeInspected)
                .Include(i => i.inspector).Include(i => i.associatedMaintenance)
                .Where( b => b.inspectionId == id).First();
        }

        public int totalAppAdmins()
        {
            return db.BikeUser.Where(m => m.canAdministerSite).Count();
        }

        public int totalCheckOutPeople()
        {
            return db.BikeUser.Where(m => m.canCheckOutBikes).Count();
        }

        public int totalMechanics()
        {
            return db.BikeUser.Where(m => m.canMaintainBikes).Where(a => a.isArchived == false).Count();
        }

        public int totalUsers()
        {
            return db.BikeUser.Where(a => a.isArchived == false).Count();
        }

        public int totalBikes()
        {
            return db.Bike.Where(a => a.isArchived == false).Count();
        }

        public int totalRacks()
        {
            return db.BikeRack.Where(a => a.isArchived == false).Count();
        }

        public int totalCheckOuts()
        {
            return db.CheckOut.Count();
        }

        public int totalInspections()
        {
            return db.Inspection.Count();
        }

        public int totalMaintenances()
        {
            return db.MaintenanceEvent.Count();
        }

        public int totalCheckedOutBikes()
        {
            return db.Bike.Include(c => c.checkOuts)
            .Where(o => o.checkOuts.Any(c => c.isResolved == false)).Count();
        }

        public int totalAvailableBikes()
        {
            return db.Bike.Include(c => c.checkOuts)
            .Where(o => o.checkOuts.All(c => c.isResolved == true)).Count();
        }

        public int totalOverdueBikes()
        {
            return db.Bike.Include(c => c.checkOuts)
            .Where(o => o.checkOuts.Any(c => c.isResolved == false)).Count();
        }


        public IEnumerable<CheckOut> getBikesCheckouts(int bikeId, int count, int skip)
        {
            return db.CheckOut.Include(b => b.bike).Include(r => r.rackCheckedIn)
                    .Include(r => r.rackCheckedOut).Include(u => u.user).Include(c => c.checkOutPerson)
                    .OrderByDescending(c => c.timeOut).Where(i => i.bike.bikeId == bikeId).Skip(skip).Take(count).ToList();
        }

        public IEnumerable<bikeUser> getFilteredUsers(int count, int skip, bool hasCharges, bool hasBike = false, bool canMaintain = false, bool canAdmin = false, bool canRide = false, bool canCheckout = false, string name = "")
        {
            if(!string.IsNullOrWhiteSpace(name))
            {
                return db.BikeUser.Where(u => u.userName.Contains(name)).ToList();
            }
            var query = db.BikeUser.Include(u => u.bike);//Include(c => c.charges)
            /*if (hasCharges)
            {
                query = query.Where(c => c.charges.Any(r => !r.isResolved));
            }*/
            if (hasBike)
            {
                query = query.Where(u => u.hasBike);
            }
            if (canMaintain)
            {
                query = query.Where(c => c.canMaintainBikes);
            }
            if (canAdmin)
            {
                query = query.Where(c => c.canAdministerSite);
            }
            if (canCheckout)
            {
                query = query.Where(c => c.canCheckOutBikes);
            }
            if (canRide)
            {
                query = query.Where(c => c.canBorrowBikes);
            }
            try
            {
                return query.OrderByDescending(u => u.userName).Skip(skip).Take(count).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.ToString());
            }
            
        }


        public void addHourToRack(Hour hour)
        {
            var rack = db.BikeRack.Find(hour.rack.bikeRackId);
            hour.rack = rack;
            db.hour.Add(hour);
            var trace = new Tracer();
            trace.rack = rack;
            trace.time = DateTime.Now;
            trace.comment = "Bike rack hours updated.";
            db.tracer.Add(trace);
            db.SaveChanges();
        }

        public void deleteHourById(int hourId)
        {
            db.hour.Remove(db.hour.Find(hourId));
            var trace = new Tracer();
            trace.rack = db.hour.Find(hourId).rack;
            trace.time = DateTime.Now;
            trace.comment = "Bike rack hours updated.";
            db.tracer.Add(trace);
            db.SaveChanges();
        }
    }
}

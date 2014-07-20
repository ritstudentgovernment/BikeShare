using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using BikeShare.Interfaces;
using BikeShare.Models;

namespace BikeShare.Repositories
{
    public class ExploreDbRepository : IExploreRepository
    {
        private BikesContext db = new BikesContext();
        public int countAvailableBikes()
        {
            var bikes = db.Bike.Include(i => i.checkOuts).Where(c => c.checkOuts.Where(z => z.isResolved == false).Count() < 1).Where(a => a.isArchived == false).ToList();
            return bikes.Count();
        }

        public IEnumerable<Bike> getAvailableBikes()
        {
            return db.Bike.Include(i => i.checkOuts).Where(c => c.checkOuts.Where(z => z.isResolved == false).Count() < 1).Where(a => a.isArchived == false).ToList();
        }

        public IEnumerable<BikeRack> getAvailableRacks()
        {
            return db.BikeRack.Include(b => b.bikes).Include(h => h.hours).Where(a => a.isArchived == false).ToList();
        }


        public IEnumerable<CheckOut> getSomeCheckouts(int skip, int count)
        {
            using (var db = new BikesContext())
            {
                return db.CheckOut.Include(b => b.bike).Include(r => r.rackCheckedIn).Include(r => r.rackCheckedOut).OrderByDescending(d => d.timeOut).Skip(skip).Take(count).ToList();
            }
        }

        public IEnumerable<MaintenanceEvent> getSomeMaintenance(int skip, int count)
        {
            using (var db = new BikesContext())
            {
                return db.MaintenanceEvent.Include(b => b.bikeAffected).OrderByDescending(d => d.timeAdded).Skip(skip).Take(count).ToList();
            }
        }

        public IEnumerable<Inspection> getSomeInspections(int skip, int count)
        {
            using (var db = new BikesContext())
            {
                return db.Inspection.Include(b => b.bike).OrderByDescending(d => d.datePerformed).Skip(skip).Take(count).ToList();
            }
        }


        public IEnumerable<Tracer> getSomeEvents(int userId, int skip, int count)
        {
            using (var db = new BikesContext())
            {
                return db.tracer.Include(u => u.user).Include(b => b.bike).Include(c => c.charge).Include(c => c.checkOut).Include(u => u.inspection)
                    .Include(m => m.maint).Include(r => r.rack).Include(s => s.shop).Include(u => u.update).Include(h => h.workHour).OrderByDescending(t => t.time)
                    .Where(u => u.user.bikeUserId == userId).Skip(skip).Take(count).ToList();
            }
        }


        public int countEventsForUser(int userId)
        {
            using (var db = new BikesContext())
            {
                return db.tracer.Where(u => u.user.bikeUserId == userId).Count();
            }
        }
    }
}

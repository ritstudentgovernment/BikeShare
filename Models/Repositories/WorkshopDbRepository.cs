using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using BikeShare.Models;
using BikeShare.Interfaces;

namespace BikeShare.Repositories
{
    public class WorkshopDbRepository : IWorkshopRepository
    {
        private BikesContext db;
        public Workshop getWorkshopById(int id)
        {
            using (db = new BikesContext())
            {
                return db.WorkShop.Include(m => m.maintenanceEvents).Include(h => h.hours).Where(i => i.workshopId == id).First();
            }
        }

        public Workshop getWorkshopByName(string name)
        {
            using (db = new BikesContext())
            {
                var possibilities = db.WorkShop.Include(m => m.maintenanceEvents).Include(h => h.hours).Where(i => i.name == name);
                if (possibilities.Count() > 1)
                {
                    return possibilities.Where(a => a.isArchived == false).First();
                }
                return possibilities.First();
            }
        }

        public void updateWorkshop(int workshopId, string name, float GPSCoordX, float GPSCoordY)
        {
            using (db = new BikesContext())
            {
                var workshop = db.WorkShop.Include(m => m.maintenanceEvents).Where(i => i.workshopId == workshopId).First();
                workshop.name = name;
                workshop.GPSCoordY = GPSCoordY;
                workshop.GPSCoordX = GPSCoordX;
                var trace = new Tracer();
                trace.shop = workshop;
                trace.time = DateTime.Now;
                trace.comment = "Updated workshop " + workshop.name;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void archiveWorkshopById(int id)
        {
            using (db = new BikesContext())
            {
                var workshop = db.WorkShop.Include(m => m.maintenanceEvents).Where(i => i.workshopId == id).First();
                workshop.isArchived = true;
                var trace = new Tracer();
                trace.shop = workshop;
                trace.time = DateTime.Now;
                trace.comment = "Archived workshop " + workshop.name;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void unarchiveWorkshopById(int id)
        {
            using (db = new BikesContext())
            {
                var workshop = db.WorkShop.Include(m => m.maintenanceEvents).Where(i => i.workshopId == id).First();
                workshop.isArchived = false;
                var trace = new Tracer();
                trace.shop = workshop;
                trace.time = DateTime.Now;
                trace.comment = "Unarchived workshop " + workshop.name;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void createNewWorkshop(string name, float GPSCoordX, float GPSCoordY)
        {
            using (db = new BikesContext())
            {
                var workshop = new Workshop();
                workshop.isArchived = false;
                workshop.name = name;
                workshop.maintenanceEvents = new List<MaintenanceEvent>();
                workshop.GPSCoordY = GPSCoordY;
                workshop.GPSCoordX = GPSCoordX;
                db.WorkShop.Add(workshop);
                var trace = new Tracer();
                trace.shop = workshop;
                trace.time = DateTime.Now;
                trace.comment = "Created new workshop " + workshop.name;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public IEnumerable<Workshop> getSomeWorkshops(int skip, int count, bool includeArchived, bool includeCurrent)
        {
            using (db = new BikesContext())
            {
                var query = db.WorkShop.Include(m => m.maintenanceEvents).Include(h => h.hours);

                if (!includeArchived && !includeCurrent)
                {
                    return new List<Workshop>();
                }
                else if (includeArchived && !includeCurrent)
                {
                    query = query.Where(a => a.isArchived == includeArchived);
                }
                else if (includeCurrent && !includeArchived)
                {
                    query = query.Where(a => a.isArchived == false);
                }
                return query.OrderByDescending(i => i.workshopId).Skip(skip).Take(count).ToList();
            }
        }

        public int countWorkshops(bool includeArchived, bool includeCurrent)
        {
            using (db = new BikesContext())
            {
                var query = db.WorkShop;

                if (!includeArchived && !includeCurrent)
                {
                    return 0;
                }
                else if (includeArchived && !includeCurrent)
                {
                    return query.Where(a => a.isArchived == includeArchived).Count();
                }
                else if (includeCurrent && !includeArchived)
                {
                    return db.WorkShop.Where(a => a.isArchived == false).Count();
                }
                return db.WorkShop.Count();
            }
        }


        public void addHour(Hour hour)
        {
            using (var db = new BikesContext())
            {
                hour.shop = db.WorkShop.Find(hour.shop.workshopId);
                db.hour.Add(hour);
                var trace = new Tracer();
                trace.shop = hour.shop;
                trace.time = DateTime.Now;
                trace.comment = "Updated workshop's hours of operation: " + trace.shop.name;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void deleteHourById(int hourId)
        {
            using (var db = new BikesContext())
            {
                var shop = db.workHours.Find(hourId).shop;
                var trace = new Tracer();
                trace.shop = shop;
                trace.time = DateTime.Now;
                trace.comment = "Updated workshop's hours of operation: " + trace.shop.name;
                db.tracer.Add(trace);
                db.hour.Remove(db.hour.Find(hourId));
                db.SaveChanges();
            }
        }
    }
}

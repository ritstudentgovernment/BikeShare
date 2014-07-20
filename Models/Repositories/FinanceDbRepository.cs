using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Interfaces;
using BikeShare.Models;
using System.Data.Entity;

namespace BikeShare.Repositories
{
    public class FinanceDbRepository : IFinanceRepository
    {
        private BikesContext db;
        public int countTotalCharges()
        {
            using (db = new BikesContext())
            {
                return db.Charge.Count();
            }
        }

        public int countTotalCharges(DateTime start, DateTime end)
        {
            using (db = new BikesContext())
            {
                return db.Charge.Where(c => c.dateAssesed > start && c.dateAssesed < end).Count();
            }
        }

        public int countResolvedCharges()
        {
            using (db = new BikesContext())
            {
                return db.Charge.Where(r => r.isResolved).Count();
            }
        }

        public int countResolvedCharges(DateTime start, DateTime end)
        {
            using (db = new BikesContext())
            {
                return db.Charge.Where(r => r.isResolved).Where(r => r.dateResolved > start && r.dateResolved < end).Count();
            }
        }

        public int countUnresolvedCharges()
        {
            using (db = new BikesContext())
            {
                return db.Charge.Where(r => !r.isResolved).Count();
            }
        }

        public decimal incomeToDate()
        {
            using (db = new BikesContext())
            {
                var model = db.Charge.Where(r => r.isResolved).Where(r => r.amountPaid != null).ToList();
                return model.Sum(r => r.amountPaid);
            }
        }

        public decimal outstandingBalance()
        {
            using (db = new BikesContext())
            {
                var charges = db.Charge.Where(r => !r.isResolved);
                if (charges.Count() > 0)
                {
                    return charges.Sum(r => r.amountCharged);
                }
                else
                {
                    return 0;
                }
            }
        }

        public IEnumerable<Models.Charge> getUnresolvedCharges()
        {
            using (db = new BikesContext())
            {
                return db.Charge.Include(c => c.user).Where(r => !r.isResolved).ToList();
            }
        }

        public IEnumerable<Models.Charge> getUnresolvedCharges(int count, int skip)
        {
            using (db = new BikesContext())
            {
                return db.Charge.Include(c => c.user).Where(r => !r.isResolved).OrderByDescending(d => d.dateAssesed).Skip(skip).Take(count).ToList();
            }
        }

        public IEnumerable<Models.Charge> getAllCharges()
        {
            using (db = new BikesContext())
            {
                return db.Charge.Include(c => c.user).ToList();
            }
        }

        public IEnumerable<Models.Charge> getAllCharges(int count, int skip)
        {
            using (db = new BikesContext())
            {
                return db.Charge.Include(c => c.user).OrderByDescending(d => d.dateAssesed).Skip(skip).Take(count).ToList();
            }
        }

        public void addCharge(Models.Charge charge)
        {
            using (db = new BikesContext())
            {
                db.Charge.Add(charge);
                var trace = new Tracer();
                trace.user = charge.user;
                trace.charge = charge;
                trace.time = DateTime.Now;
                trace.comment = "User was charged";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void updateCharge(Models.Charge charge)
        {
            using (db = new BikesContext())
            {
                var oldCharge = db.Charge.Find(charge.chargeId);
                oldCharge.amountCharged = charge.amountCharged;
                oldCharge.amountPaid = charge.amountPaid;
                oldCharge.dateAssesed = charge.dateAssesed;
                oldCharge.dateResolved = oldCharge.dateResolved;
                oldCharge.description = charge.description;
                oldCharge.isResolved = charge.isResolved;
                oldCharge.notificationsCounter = charge.notificationsCounter;
                oldCharge.title = charge.title;
                var trace = new Tracer();
                trace.user = charge.user;
                trace.charge = charge;
                trace.time = DateTime.Now;
                trace.comment = "User updated charge";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }


        public void addCharge(decimal amount, string userName, string title, string description)
        {
            using (db = new BikesContext())
            {
                var charge = new Charge();
                charge.user = db.BikeUser.Where(u => u.userName == userName).First();
                charge.amountCharged = amount;
                charge.title = title;
                charge.description = description;
                charge.isResolved = false;
                charge.dateAssesed = DateTime.Now;
                charge.dateResolved = DateTime.Now;
                db.Charge.Add(charge);
                var trace = new Tracer();
                trace.user = charge.user;
                trace.charge = charge;
                trace.time = DateTime.Now;
                trace.comment = "User was charged";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }


        public void closeCharge(int chargeId, decimal amountPaid)
        {
            using(db = new BikesContext())
            {
                var oldCharge = db.Charge.Find(chargeId);
                var user = db.BikeUser.Find(oldCharge.user.bikeUserId);
                oldCharge.user = user;
                oldCharge.amountPaid = amountPaid;
                oldCharge.dateResolved = DateTime.Now;
                oldCharge.isResolved = true;
                var trace = new Tracer();
                trace.user = oldCharge.user;
                trace.charge = oldCharge;
                trace.time = DateTime.Now;
                trace.comment = "User charge was closed.";
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }


        public void updateCharge(int chargeId, decimal amountCharged, string title, string description)
        {
            using (db = new BikesContext())
            {
                var oldCharge = db.Charge.Find(chargeId);
                var user = db.BikeUser.Find(oldCharge.user.bikeUserId);
                oldCharge.amountCharged = amountCharged;
                oldCharge.title = title;
                var trace = new Tracer();
                trace.user = oldCharge.user;
                trace.charge = oldCharge;
                trace.time = DateTime.Now;
                trace.comment = "User updated charge";
                db.tracer.Add(trace);
                oldCharge.description = description;
                db.SaveChanges();
            }
        }

    }
}

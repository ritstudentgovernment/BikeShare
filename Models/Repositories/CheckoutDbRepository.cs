using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;
using System.Data.Entity;
using BikeShare.Interfaces;
using System.Net.Mail;
using System.Configuration;


namespace BikeShare.Repositories
{
    public class CheckoutDbRepository : ICheckOutRepository
    {
        public IEnumerable<Bike> getAvailableBikesForRack(int rackId)
        {
            using(var db = new BikesContext())
            {
                var query = db.Bike.Include(l => l.bikeRack).Include(c => c.checkOuts).Where(c => c.checkOuts.All(r => r.isResolved)).Where(r => r.bikeRack.bikeRackId == rackId).Where(a => !a.isArchived);
                var maint = db.MaintenanceEvent.Include(b => b.bikeAffected).Where(r => !r.resolved).Where(d => d.disableBike).Select(b => b.bikeAffected.bikeId).ToList();
                var spec = db.Inspection.Include(b => b.bike).Include(b => b.bike.bikeRack).Where(b => b.bike.bikeRack.bikeRackId == rackId);
                List<Bike> result = new List<Bike>();
                var inspectionPeriod = (int)db.settings.First().DaysBetweenInspections;
                List<Bike> temp = query.Where(b => !maint.Contains(b.bikeId)).ToList().Where(l => { DateTime x = (DateTime)l.lastPassedInspection; return x.AddDays(inspectionPeriod) > DateTime.Now; }).ToList();
                foreach(var bike in temp)
                {
                    if (spec.Where( b => b.bike.bikeId == bike.bikeId).OrderByDescending(d => d.datePerformed).First().isPassed == true)
                    {
                        result.Add(bike);
                    }
                }
                return result;
            }
        }

        public IEnumerable<Bike> getCheckedOutBikes()
        {
            using (var db = new BikesContext())
            {
                return db.Bike.Include(l => l.bikeRack).Include(c => c.checkOuts).Where(c => c.checkOuts.Any(r => !r.isResolved)).ToList();
            }
        }

        public BikeRack getRackById(int rackId)
        {
            using (var db = new BikesContext())
            {
                return db.BikeRack.Include(c => c.checkOuts).Where(i => i.bikeRackId == rackId).First();
            }
        }

        public string checkOutBike(int bikeId, string userName, string checkOutPerson, int rackId)
        {
            using (var db = new BikesContext())
            {
                try
                {
                    Bike bike = db.Bike.Include(l => l.bikeRack).Include(c => c.checkOuts).Where(i => i.bikeId == bikeId).First();
                    bike.bikeRack = db.BikeRack.Find(rackId);
                    bike.lastCheckedOut = DateTime.Now;
                    CheckOut check = new CheckOut();
                    check.bike = db.Bike.Find(bikeId);
                    check.isResolved = false;
                    check.rackCheckedOut = db.BikeRack.Find(rackId);
                    check.timeOut = DateTime.Now;
                    check.timeIn = DateTime.Now; //TODO - don't do this
                    bikeUser rider = db.BikeUser.Where(n => n.userName == userName).First();
                    bikeUser dcheckOutPerson = db.BikeUser.Where(n => n.userName == checkOutPerson).First();
                    int days = db.settings.First().daysBetweenRegistrations;
                    if (rider.lastRegistered.AddDays(days) < DateTime.Now)
                    {
                        return "User isn't registered.";
                    }
                    check.user = rider;
                    check.checkOutPerson = dcheckOutPerson;
                    rider.bike = bike;
                    rider.hasBike = true;
                    rider.checkOuts.Add(check);
                    db.CheckOut.Add(check);
                    var trace = new Tracer();
                    trace.checkOut = check;
                    trace.user = check.user;
                    trace.time = DateTime.Now;
                    trace.comment = "User Checked out Bike";
                    db.tracer.Add(trace);
                    db.SaveChanges();
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
                catch(System.InvalidOperationException)
                {
                    return "Checkout Failed - User Does Not Exist.";
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException)
                {
                    return "Checkout Failed - Validation Error";
                }
                
            }
            return "Bike checked out! :)";
        }

        public string checkInBike(int bikeId, string checkOutPerson, int rackId)
        {
            using (var db = new BikesContext())
            {
                try
                {
                    Bike bike = db.Bike.Include(l => l.bikeRack).Include(c => c.checkOuts).Where(i => i.bikeId == bikeId).First();
                    bike.bikeRack = db.BikeRack.Find(rackId);
                    CheckOut check = db.CheckOut.Include(b => b.bike).Include(u => u.checkOutPerson).Include(u => u.user).Include(r => r.rackCheckedIn).Include(r => r.rackCheckedOut).Where(r => !r.isResolved).Where(b => b.bike.bikeId == bikeId).First();
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
                    db.tracer.Add(trace);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException)
                {
                    return "Check-in Failed - Validation Error";
                }

            }
            return "Bike checked in! :)";
        }


        public IEnumerable<BikeRack> getAllRacks()
        {
            using (var db = new BikesContext())
            {
                return db.BikeRack.ToList();
            }
        }


        public IEnumerable<Bike> getUnavailableBikesForRack(int rackId)
        {
            using (var db = new BikesContext())
            {
                var query = db.Bike.Include(l => l.bikeRack).Where(r => r.bikeRack.bikeRackId == rackId).Where(a => !a.isArchived);
                var available = getAvailableBikesForRack(rackId).ToList();
                List<Bike> temp = query.Where(b => !available.Contains(b)).ToList();

                return temp;
            }
        }
    }
}

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
    public class UserDbRepository : IUserRepository
    {
        public IEnumerable<bikeUser> getSomeUsers(int count, int skip = 0, bool includeCheckOut = true, bool includeRiders = true, bool includeMechanics = true, bool includeAdmins = true, bool includeArchived = false, bool includeCurrent = true)
        {
            using (var db = new BikesContext())
            {
                var query = db.BikeUser.Include(b => b.bike).Include(c => c.charges).Include(c => c.checkOuts)
                    .Include(m => m.maintenanceEvents);
                if (!includeCheckOut)
                {
                    query = query.Where(p => p.canCheckOutBikes == false);
                }
                if (!includeRiders)
                {
                    query = query.Where(p => p.canBorrowBikes == false);
                }
                if (!includeMechanics)
                {
                    query = query.Where(p => p.canMaintainBikes == false);
                }
                if (!includeAdmins)
                {
                    query = query.Where(p => p.canAdministerSite == false);
                }
                if (!includeArchived)
                {
                    query = query.Where(a => a.isArchived == false);
                }
                if(!includeCurrent)
                {
                    query = query.Where(a => a.isArchived == true);
                }
                return query.OrderByDescending(d => d.lastRegistered).Skip(skip).Take(count).ToList();
            }
        }

        public IEnumerable<bikeUser> getUsersWithBalances(int count = 100000, int skip = 0)
        {
            using (var db = new BikesContext())
            {
                return db.BikeUser.Include(b => b.bike).Include(c => c.charges).Include(c => c.checkOuts)
                    .Include(m => m.maintenanceEvents).Where(c => c.charges.Any(i => i.isResolved == false))
                    .OrderByDescending(d => d.lastRegistered).Skip(skip).Take(count).ToList();
            }
        }

        public void archiveUser(int bikeUserId)
        {
            using (var db = new BikesContext())
            {
                var user = db.BikeUser.Include(b => b.bike).Include(c => c.charges).Include(c => c.checkOuts)
                    .Include(m => m.maintenanceEvents).Where(i => i.bikeUserId == bikeUserId).First();
                user.isArchived = true;
                var trace = new Tracer();
                trace.user = user;
                trace.time = DateTime.Now;
                trace.comment = "Archived User: " + user.userName;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void unArchiveUser(int bikeUserId)
        {
            using (var db = new BikesContext())
            {
                var user = db.BikeUser.Include(b => b.bike).Include(c => c.charges).Include(c => c.checkOuts)
                    .Include(m => m.maintenanceEvents).Where(i => i.bikeUserId == bikeUserId).First();
                user.isArchived = false;
                var trace = new Tracer();
                trace.user = user;
                trace.time = DateTime.Now;
                trace.comment = "Unarchived User: " + user.userName;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void updateUser(int id, string name, string email, string phone, string firstName = null, string lastName = null)
        {
            using (var db = new BikesContext())
            {
                var user = db.BikeUser.Include(b => b.bike).Include(c => c.charges).Include(c => c.checkOuts)
                   .Include(m => m.maintenanceEvents).Where(i => i.bikeUserId == id).First();
                user.userName = name;
                user.firstName = firstName;
                user.lastName = lastName;
                user.email = email;
                user.phoneNumber = phone;
                var trace = new Tracer();
                trace.user = user;
                trace.time = DateTime.Now;
                trace.comment = "Updated User: " + user.userName;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public void promoteUser(int userId, bool canCheckOut, bool canBorrow, bool canMaintain, bool canManageApp)
        {
            using (var db = new BikesContext())
            {
                var user = db.BikeUser.Include(b => b.bike).Include(c => c.charges).Include(c => c.checkOuts)
                    .Include(m => m.maintenanceEvents).Where(i => i.bikeUserId == userId).First();
                user.canCheckOutBikes = canCheckOut;
                user.canBorrowBikes = canBorrow;
                user.canMaintainBikes = canMaintain;
                user.canAdministerSite = canManageApp;
                var trace = new Tracer();
                trace.user = user;
                trace.time = DateTime.Now;
                trace.comment = "Managed User's Permissions: " + user.userName;
                db.tracer.Add(trace);
                db.SaveChanges();
            }

        }

        public void createuser(string name, string email, string phone = null, bool canCheckOut = false, bool canBorrow = false, bool canMaintain = false, bool canManageApp = false, string firstName = null, string lastName = null)
        {
            using (var db = new BikesContext())
            {
                if (db.BikeUser.Where(u => u.userName == name).Count() > 0)
                {
                    throw new InvalidOperationException("User with same name already exists. :(");
                }
                var user = new bikeUser();
                user.bike = null;
                user.charges = new List<Charge>();
                user.checkOuts = new List<CheckOut>();
                user.email = email;
                user.firstName = firstName;
                user.lastName = lastName;
                user.hasBike = false;
                user.isArchived = false;
                user.lastRegistered = new DateTime(2000, 1, 1);
                user.maintenanceEvents = new List<MaintenanceEvent>();
                user.maintenanceUpdates = new List<MaintenanceUpdate>();
                user.phoneNumber = phone;
                user.canAdministerSite = canManageApp;
                user.canBorrowBikes = canBorrow;
                user.canCheckOutBikes = canCheckOut;
                user.canMaintainBikes = canMaintain;
                user.userName = name;
                db.BikeUser.Add(user);
                var trace = new Tracer();
                trace.user = user;
                trace.time = DateTime.Now;
                trace.comment = "Created User: " + user.userName;
                db.tracer.Add(trace);
                db.SaveChanges();
            }
        }

        public bikeUser getUserById(int id)
        {
            using (var db = new BikesContext())
            {
                return db.BikeUser.Include(b => b.bike).Include(c => c.charges).Include(c => c.checkOuts)
                    .Include(m => m.maintenanceEvents).Where(i => i.bikeUserId == id).First();
            }
        }

        public int totalAppAdmins(bool includeArchived = false, bool includeCurrent = true)
        {
            using (var db = new BikesContext())
            {
                if (includeCurrent && includeArchived)
                {
                    return db.BikeUser.Where(p => p.canAdministerSite).Count();
                }
                else if (!includeArchived && includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canAdministerSite).Where(a => !a.isArchived).Count();
                }
                else if (includeArchived && !includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canAdministerSite).Where(a => a.isArchived).Count();
                }
                return 0;
            }
        }

        public int totalCheckOutPeople(bool includeArchived = false, bool includeCurrent = true)
        {
            using (var db = new BikesContext())
            {
                if (includeCurrent && includeArchived)
                {
                    return db.BikeUser.Where(p => p.canCheckOutBikes).Count();
                }
                else if (!includeArchived && includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canCheckOutBikes).Where(a => !a.isArchived).Count();
                }
                else if (includeArchived && !includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canCheckOutBikes).Where(a => a.isArchived).Count();
                }
                return 0;
            }
        }

        public int totalMechanics(bool includeArchived = false, bool includeCurrent = true)
        {
            using (var db = new BikesContext())
            {
                if (includeCurrent && includeArchived)
                {
                    return db.BikeUser.Where(p => p.canMaintainBikes).Count();
                }
                else if (!includeArchived && includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canMaintainBikes).Where(a => !a.isArchived).Count();
                }
                else if (includeArchived && !includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canMaintainBikes).Where(a => a.isArchived).Count();
                }
                return 0;
            }
        }
        public int totalRiders(bool includeArchived = false, bool includeCurrent = true)
        {
            using (var db = new BikesContext())
            {
                if (includeCurrent && includeArchived)
                {
                    return db.BikeUser.Where(p => p.canBorrowBikes).Count();
                }
                else if (!includeArchived && includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canBorrowBikes).Where(a => !a.isArchived).Count();
                }
                else if (includeArchived && !includeCurrent)
                {
                    return db.BikeUser.Where(p => p.canBorrowBikes).Where(a => a.isArchived).Count();
                }
                return 0;
            }
        }

        public int totalUsers(bool includeArchived = false, bool includeCurrent = true)
        {
            using (var db = new BikesContext())
            {
                if (includeCurrent && includeArchived)
                {
                    return db.BikeUser.Count();
                }
                else if (!includeArchived && includeCurrent)
                {
                    return db.BikeUser.Where(a => !a.isArchived).Count();
                }
                else if (includeArchived && !includeCurrent)
                {
                    return db.BikeUser.Where(a => a.isArchived).Count();
                }
                return 0;
            }
        }

        public bool canUserManageApp(string name)
        {
            using (var db =  new BikesContext())
            {
                return db.BikeUser.Where(u => u.userName == name).First().canAdministerSite;
            }
        }

        public bikeUser getUserByName(string name)
        {
            using (var db = new BikesContext())
            {
                return db.BikeUser.Include(c => c.charges).Include(c => c.checkOuts)
                    .Include(m => m.maintenanceEvents).Where(i => i.userName == name).First();
            }
        }


        public void registerUser(string userName, string firstName, string lastName)
        {
            using (var db = new BikesContext())
            {
                var user = db.BikeUser.Where(u => u.userName == userName).First();
                user.firstName = firstName;
                user.lastName = lastName;
                user.lastRegistered = DateTime.Now;
                db.SaveChanges();
            }
        }


        public bool isUserRegistrationValid(string name)
        {
            using (var db = new BikesContext())
            {
                var user = db.BikeUser.Where(n => n.userName == name).First();
                var setting = db.settings.First();
                if (user.lastRegistered.AddDays(setting.daysBetweenRegistrations) < DateTime.Now)
                {
                    return false;
                }
                return true;
            }
        }

        public bool isUserRegistrationValid(int id)
        {
            using (var db = new BikesContext())
            {
                var user = db.BikeUser.Find(id);
                var setting = db.settings.First();
                if (user.lastRegistered.AddDays(setting.daysBetweenRegistrations) < DateTime.Now)
                {
                    return false;
                }
                return true;
            }
        }


        public bool doesUserExist(string name)
        {
            using (var db = new BikesContext())
            {
                if (db.BikeUser.Where(u => u.userName == name).Count() < 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}

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
    public class SettingsDbRepository : ISettingRepository
    {
        public string getappName()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().appName;
            }
        }

        public string getexpectedEmail()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().expectedEmail;
            }
        }

        public string getAdminEmails()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().adminEmailList;
            }
        }

        public int getmaxRentDays()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().maxRentDays;
            }
        }

        public int getDaysBetweenInspections()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().DaysBetweenInspections;
            }
        }

        public void setAppName(string name)
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                settings.appName = name;
                db.SaveChanges();
            }
        }

        public void setExpectedEmail(string email)
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                settings.expectedEmail = email;
                db.SaveChanges();
            }
        }

        public void setAdminEmails(string emails)
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                settings.adminEmailList = emails;
                db.SaveChanges();
            }
        }

        public void setMaxRentDays(int days)
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                settings.maxRentDays = days;
                db.SaveChanges();
            }
        }

        public void setDaysBetweenInspections(int days)
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                settings.DaysBetweenInspections = days;
                db.SaveChanges();
            }
        }


        public void setDaysBetweenRegistrations(int days)
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                settings.daysBetweenRegistrations = days;
                db.SaveChanges();
            }
        }


        public int getDaysBetweenRegistrations()
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                return settings.daysBetweenRegistrations;
            }
        }

        public void setOverdueBikeMailingInterval(int hours)
        {
            using (var db = new BikesContext())
            {
                var settings = db.settings.First();
                settings.overdueBikeMailingIntervalHours = hours;
                db.SaveChanges();
            }
        }

        public int getOverdueBikeMailingInterval()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().overdueBikeMailingIntervalHours;
            }
        }
    }
}

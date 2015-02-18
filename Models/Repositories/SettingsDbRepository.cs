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

        public string getFooterHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().footerHTML;
            }
        }

        public void setFooterHTML(string footerHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().footerHTML = footerHTML;
                db.SaveChanges();
            }
        }
        public string getHomeHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().homeHTML;
            }
        }

        public void setHomeHTML(string homeHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().homeHTML = homeHTML;
                db.SaveChanges();
            }
        }
        public string getAnnouncementHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().announcementHTML;
            }
        }

        public void setAnnouncementHTML(string announcementHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().announcementHTML = announcementHTML;
                db.SaveChanges();
            }
        }
        public string getFAQHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().FAQHTML;
            }
        }

        public void setFAQHTML(string FAQHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().FAQHTML = FAQHTML;
                db.SaveChanges();
            }
        }
        public string getContactHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().contactHTML;
            }
        }

        public void setContactHTML(string contactHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().contactHTML = contactHTML;
                db.SaveChanges();
            }
        }
        public string getAboutHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().aboutHTML;
            }
        }

        public void setAboutHTML(string aboutHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().aboutHTML = aboutHTML;
                db.SaveChanges();
            }
        }
        public string getSafetyHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().safetyHTML;
            }
        }

        public void setSafetyHTML(string safetyHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().safetyHTML = safetyHTML;
                db.SaveChanges();
            }
        }


        public string getRegisterHTML()
        {
            using (var db = new BikesContext())
            {
                return db.settings.First().registerHTML;
            }
        }

        public void setRegisterHTML(string registerHTML)
        {
            using (var db = new BikesContext())
            {
                db.settings.First().registerHTML = registerHTML;
                db.SaveChanges();
            }
        }
    }
}

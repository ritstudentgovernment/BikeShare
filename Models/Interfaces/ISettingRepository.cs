using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeShare.Interfaces
{
    public interface ISettingRepository
    {
        string getappName();
        string getexpectedEmail();
        string getAdminEmails();
        int getmaxRentDays();
        int getDaysBetweenInspections();
        int getDaysBetweenRegistrations();
        void setAdminEmails(string emails);
        void setAppName(string name);
        void setExpectedEmail(string email);
        void setMaxRentDays(int days);
        void setDaysBetweenInspections(int days);
        void setDaysBetweenRegistrations(int days);

        void setOverdueBikeMailingInterval(int hours);

        int getOverdueBikeMailingInterval();
        string getFooterHTML();
        string getHomeHTML();
        string getAnnouncementHTML();
        void setFooterHTML(string footerHTML);
        void setHomeHTML(string homeHTML);
        void setAnnouncementHTML(string announcementHTML);
        void setFAQHTML(string FAQHTML);
        void setAboutHTML(string aboutHTML);
        void setSafetyHTML(string safetyHTML);
        void setContactHTML(string contactHTML);
        string getContactHTML();
        string getSafetyHTML();
        string getAboutHTML();
        string getFAQHTML();
        string getRegisterHTML();
        void setRegisterHTML(string registerHTML);
    }
}

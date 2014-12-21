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
        int getmaxRentDays();
        int getDaysBetweenInspections();
        int getDaysBetweenRegistrations();
        void setAppName(string name);
        void setExpectedEmail(string email);
        void setMaxRentDays(int days);
        void setDaysBetweenInspections(int days);
        void setDaysBetweenRegistrations(int days);

        void setOverdueBikeMailingInterval(int hours);

        int getOverdueBikeMailingInterval();
    }
}

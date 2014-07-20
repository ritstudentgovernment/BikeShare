using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;

namespace BikeShare.Interfaces
{
    public interface IWorkshopRepository
    {
        Workshop getWorkshopById(int id);
        Workshop getWorkshopByName(string name);
        void updateWorkshop(int workshopId, string name, float GPSCoordX, float GPSCoordY);
        void archiveWorkshopById(int id);
        void unarchiveWorkshopById(int id);
        void createNewWorkshop(string name, float GPSCoordX, float GPSCoordY);
        IEnumerable<Workshop> getSomeWorkshops(int skip, int count, bool includeArchived, bool includeCurrent);
        int countWorkshops(bool includeArchived, bool includeCurrent);
        void addHour(Hour hour);
        void deleteHourById(int hourId);
    }
}

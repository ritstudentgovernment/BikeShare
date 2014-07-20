using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;

namespace BikeShare.Interfaces
{
    public interface IMaintenanceRepository
    {
        int newMaintenance(int bikeId, string title, string details, string userName, int workshopId, bool disableBike);
        Bike getBikeById(int bikeId);
        MaintenanceEvent getMaintenanceById(int maintId);
        IEnumerable<MaintenanceEvent> getSomeMaintenance(int skip, int count, bool unResolvedOnly);
        void commentOnMaint(int maintId, string title, string body, string userName);
        void closeMaint(int maintId);
        void archiveMaint(int maintId);
        int newInspection(int bikeId, string userName, int workshopId, bool isPassed, string comment);
        void addMaintToInspection(int maintId, int inspectionId);
        IEnumerable<Inspection> getSomeInspections(int skip, int count, bool includePassed, bool includeFailed);
        IEnumerable<Inspection> getInspectionsForBike(int bikeId, int skip, int count, bool includePassed, bool includeFailed);
        IEnumerable<MaintenanceEvent> getMaintenanceForBike(int bikeId, int skip, int count, bool unResolvedOnly);
        int totalInspectionsForBike(int bikeId);
        int totalMaintForBike(int bikeId);
        IEnumerable<Bike> getSomeBikes(int skip, int count);
        IEnumerable<Workshop> getAllWorkshops();
        Inspection getInspectionById(int specId);
        IEnumerable<WorkHour> getAllHoursForUser(int userId);
        IEnumerable<WorkHour> getHoursForUser(int userId, int skip, int take);
        void recordHours(int userId, DateTime start, DateTime end, string comment, int? bikeId, int? maintenanceId, int? workshopId);
        int countHoursForUser(int userId);
    }
}

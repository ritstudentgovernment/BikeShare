using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;

namespace BikeShare.Interfaces
{
    public interface IAdminRepository
    {

        IEnumerable<Bike> getAllBikes();
        IEnumerable<Bike> getSomeBikes(int count, int skip);
        IEnumerable<Bike> getOverdueBikes(int count, int skip);
        IEnumerable<BikeRack> getAllBikeRacks();
        IEnumerable<BikeRack> getSomeBikeRacks(int count, int skip);
        IEnumerable<bikeUser> getAllUsers(bool includeCheckOut, bool includeRiders, bool includeMechanics, bool includeAdmins);
        IEnumerable<bikeUser> getSomeUsers(int count, int skip, bool includeCheckOut, bool includeRiders, bool includeMechanics, bool includeAdmins);
        IEnumerable<CheckOut> getAllCheckouts();
        IEnumerable<CheckOut> getSomeCheckouts(int count, int skip, bool excludeResolved);
        IEnumerable<CheckOut> getBikesCheckouts(int bikeId, int count, int skip);
        IEnumerable<Workshop> getAllWorkshops();
        IEnumerable<Workshop> getSomeWorkshops(int count, int skip);
        IEnumerable<Inspection> getSomeInspections(int count, int skip);
        IEnumerable<MaintenanceEvent> getAllMaintenance();
        IEnumerable<MaintenanceEvent> getSomeMaintenance(int count, int skip);
        IEnumerable<MaintenanceEvent> getMaintenanceForBike(int count, int skip, int bikeId);
        IEnumerable<MaintenanceEvent> getMaintenanceForUser(int count, int skip, int userId);
        IEnumerable<MaintenanceEvent> getMaintenanceForWorkshop(int count, int skip, int workshopId);
        IEnumerable<MaintenanceUpdate> getUpdatesForMaintenance(int count, int skip, int maintId);
        IEnumerable<bikeUser> getUsersWithBalances(int count, int skip);
        IEnumerable<bikeUser> getArchivedUsers(int count, int skip);
        IEnumerable<bikeUser> getFilteredUsers(int count, int skip, bool hasCharges, bool hasBike = false, bool canMaintain = false, bool canAdmin = false, bool canRide = false, bool canCheckout = false, string name = "");
        IEnumerable<BikeRack> getArchivedRacks(int count, int skip);
        IEnumerable<Bike> getArchivedBikes(int count, int skip);
        void archiveBike(int bikeId);
        void archiveUser(int bikeUserid);
        void archiveBikeRack(int rackId);
        void archiveMaintenanceEvent(int rackId);
        void addBike(Bike bike);
        void addBikeRack(BikeRack rack);
        void updateUser(bikeUser user);
        void promoteUser(int userId, bool canCheckOut, bool canBorrow, bool canMaintain, bool canManageApp);
        void updateBike(Bike bike);
        void updateBikeRack(BikeRack rack);
        void updateWorkshop(Workshop shop);
        void updateCharge(Charge charge);
        void addHourToRack(Hour hour);
        void deleteHourById(int hourId);
        Bike getBikeById(int id);
        BikeRack getRackById(int id);
        bikeUser getUserById(int id);
        Workshop getWorkshopById(int id);
        MaintenanceEvent getMaintenanceById(int id);
        MaintenanceUpdate getMaintenanceUpdateById(int id);
        Charge getChargeById(int id);
        Inspection getInspectionById(int id);
        int totalAppAdmins();
        int totalCheckOutPeople();
        int totalMechanics();
        int totalUsers();
        int totalBikes();
        int totalRacks();
        int totalCheckOuts();
        int totalInspections();
        int totalMaintenances();
        int totalCheckedOutBikes();
        int totalAvailableBikes();
        int totalOverdueBikes();
    }
}

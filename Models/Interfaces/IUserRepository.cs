using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;


namespace BikeShare.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<bikeUser> getSomeUsers(int count, int skip = 0, bool includeCheckOut = true, bool includeRiders = true, bool includeMechanics = true, bool includeAdmins = true, bool includeArchived = false, bool includeCurrent = true);
        IEnumerable<bikeUser> getUsersWithBalances(int count = 100000, int skip = 0);
        void archiveUser(int bikeUserId);
        void unArchiveUser(int bikeUserId);
        void updateUser(int id, string name, string email, string phone);
        void promoteUser(int userId, bool canCheckOut, bool canBorrow, bool canMaintain, bool canManageApp);
        void createuser(string name, string email, string phone, bool canCheckOut = false, bool canBorrow = false, bool canMaintain = false, bool canManageApp = false);
        void registerUser(string userName);
        bikeUser getUserById(int id);
        bikeUser getUserByName(string name);
        int totalAppAdmins(bool includeArchived = false, bool includeCurrent = true);
        int totalCheckOutPeople(bool includeArchived = false, bool includeCurrent = true);
        int totalMechanics(bool includeArchived = false, bool includeCurrent = true);
        int totalUsers(bool includeArchived = false, bool includeCurrent = true);
        bool canUserManageApp(string name);
    }
}

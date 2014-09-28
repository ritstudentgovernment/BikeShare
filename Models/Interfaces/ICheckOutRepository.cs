using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;

namespace BikeShare.Interfaces
{
    public interface ICheckOutRepository
    {
        IEnumerable<Bike> getAvailableBikesForRack(int rackId);
        IEnumerable<Bike> getCheckedOutBikes();
        IEnumerable<Bike> getUnavailableBikesForRack(int rackId);
        BikeRack getRackById(int rackId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bikeId"></param>
        /// <param name="userName"></param>
        /// <param name="checkOutPerson"></param>
        /// <param name="rackId"></param>
        /// <returns>Returns message about the success of the operation.</returns>
        string checkOutBike(int bikeId, string userName, string checkOutPerson, int rackId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bikeId"></param>
        /// <param name="checkOutPerson"></param>
        /// <param name="rackId"></param>
        /// <returns>Returns message about the success of the operation.</returns>
        string checkInBike(int bikeId, string checkOutPerson, int rackId);

        IEnumerable<BikeRack> getAllRacks();
    }
}

using BikeShare.Models;
using System.Collections.Generic;

namespace BikeShare.ViewModels
{
    public class bikeListingViewModel
    {
        public int countAvailableBikes { get; set; }

        public int rentedBikes { get; set; }

        public IEnumerable<Bike> allAvailableBikes { get; set; }

        public PageInfo pagingInfo { get; set; }
    }
}
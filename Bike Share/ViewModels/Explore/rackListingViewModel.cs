using BikeShare.Models;
using System.Collections.Generic;

namespace BikeShare.ViewModels
{
    public class rackListingViewModel
    {
        public int countBikeRacks { get; set; }

        public IEnumerable<BikeRack> bikeRacks { get; set; }

        public PageInfo pagingInfo { get; set; }
    }
}
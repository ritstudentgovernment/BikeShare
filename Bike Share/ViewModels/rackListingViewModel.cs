using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class rackListingViewModel
    {
        public int countBikeRacks { get; set; }
        public IEnumerable<BikeRack> bikeRacks { get; set; }
        public PageInfo pagingInfo { get; set; }
    }
}
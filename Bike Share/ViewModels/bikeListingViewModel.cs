using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

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
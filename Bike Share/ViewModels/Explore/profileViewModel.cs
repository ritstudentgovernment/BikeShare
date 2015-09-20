using BikeShare.Models;
using System;
using System.Collections.Generic;

namespace BikeShare.ViewModels
{
    public class profileViewModel
    {
        public bikeUser user { get; set; }

        public BikeShare.ViewModels.PageInfo pagingInfo { get; set; }

        public Boolean hasRental { get; set; }

        public int hoursLeft { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class profileViewModel
    {
        public bikeUser user { get; set; }
        public List<BikeShare.ViewModels.ActivityCard> cards { get; set; }
        public BikeShare.ViewModels.PageInfo pagingInfo { get; set; }
    }
}
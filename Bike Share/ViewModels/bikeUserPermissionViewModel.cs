using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class bikeUserPermissionViewModel
    {
        public string userName { get; set; }
        public int userId { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public Boolean canCheckOutBikes { get; set; }
        public Boolean canBorrowBikes { get; set; }
        public Boolean canMaintainBikes { get; set; }
        public Boolean canManageApp { get; set; }
    }
}
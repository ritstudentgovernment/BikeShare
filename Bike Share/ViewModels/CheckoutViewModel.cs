using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class CheckoutViewModel
    {
        public IEnumerable<Bike> availableBikes {get; set;}
        public IEnumerable<Bike> checkedOutBikes { get; set; }
        public IEnumerable<Bike> unavailableBikes { get; set; }
        public BikeRack currentRack { get; set; }
        public string errorMessage { get; set; }
        public int selectedBikeForCheckout { get; set; }
        public string userToCheckIn { get; set; }
        public string checkOutPerson { get; set; }
    }
}
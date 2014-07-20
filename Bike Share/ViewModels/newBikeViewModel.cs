using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class newBikeViewModel
    {
        public int bikeRackId { get; set; }
        public string bikeName { get; set; }
        public int bikeNumber { get; set; }
    }
}
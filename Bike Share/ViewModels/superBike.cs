using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;
using System.ComponentModel.DataAnnotations;

namespace BikeShare.ViewModels
{
    public class superBike
    {
        public Bike bike { get; set; }
        public List<MaintenanceEvent> maintenance { get; set; }
        public List<Inspection> inspections { get; set; }
    }
}
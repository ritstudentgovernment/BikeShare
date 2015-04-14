using BikeShare.Models;
using System.Collections.Generic;

namespace BikeShare.ViewModels
{
    public class superBike
    {
        public Bike bike { get; set; }

        public List<MaintenanceEvent> maintenance { get; set; }

        public List<Inspection> inspections { get; set; }
    }
}
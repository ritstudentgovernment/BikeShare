using BikeShare.Models;
using System.Collections.Generic;

namespace BikeShare.ViewModels.Maint
{
    public class dashboardVM
    {
        public IEnumerable<Bike> allBikes { get; set; }

        public IEnumerable<MaintenanceEvent> latestMaintenances { get; set; }

        public IEnumerable<Inspection> latestInspections { get; set; }
    }
}
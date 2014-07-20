using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels.Maint
{
    public class dashboardVM
    {
        public IEnumerable<Bike> allBikes { get; set; }
        public IEnumerable<MaintenanceEvent> latestMaintenances { get; set; }
        public IEnumerable<Inspection> latestInspections { get; set; }
    }
}
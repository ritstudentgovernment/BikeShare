using BikeShare.Models;
using System.Collections.Generic;

namespace BikeShare.ViewModels.Maint
{
    public class bikeDetailsVM
    {
        public string bikeName { get; set; }

        public int bikeNumber { get; set; }

        public int bikeId { get; set; }

        public IEnumerable<MaintenanceEvent> maints { get; set; }

        public int totalCheckouts { get; set; }

        public int totalInspections { get; set; }

        public IEnumerable<Inspection> inspections { get; set; }
    }
}
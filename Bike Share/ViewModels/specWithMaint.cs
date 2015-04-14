using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class specWithMaint
    {
        public Inspection spec { get; set; }

        public MaintenanceEvent maint { get; set; }
    }
}
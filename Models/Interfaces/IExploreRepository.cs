using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;

namespace BikeShare.Interfaces
{
    public interface IExploreRepository
    {
        int countAvailableBikes();
        IEnumerable<Bike> getAvailableBikes();
        IEnumerable<BikeRack> getAvailableRacks();
        IEnumerable<CheckOut> getSomeCheckouts(int skip, int count);
        IEnumerable<MaintenanceEvent> getSomeMaintenance(int skip, int count);
        IEnumerable<Inspection> getSomeInspections(int skip, int count);
        IEnumerable<Tracer> getSomeEvents(int userId, int skip, int count);
        int countEventsForUser(int userId);
    }
}

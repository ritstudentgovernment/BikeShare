using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeShare.ViewModels.Admin
{
    public class AdminBikeDetailsVM : AdminBikeVM
    {
        public class inspection
        {
            public DateTime Performed { get; set; }
            public bool Passed { get; set; }
            public string Comment { get; set; }
        }

        public class maintenance
        {
            public string Title { get; set; }
            public bool Resolved { get; set; }
            public DateTime Date { get; set; }
        }

        public string RackLastSeen { get; set; }
        public int CountOfInspections { get; set; }
        public int CountOfMaintenance { get; set; }
        public int CountOfRentals { get; set; }
        public List<inspection> Inspections { get; set; }
        public List<maintenance> Maintenance { get; set; }
    }
}

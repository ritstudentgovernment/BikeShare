using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeShare.ViewModels.Admin
{
    public class AdminUserDetailsVM : AdminUserVM
    {
        public class rental {
            public DateTime Start {get; set;}
            public DateTime? End {get; set;}
            public int BikeNumber {get; set;}
        }
        public bool IsAdmin { get; set; }
        public bool IsMechanic { get; set; }
        public bool IsCheckout { get; set; }
        public bool IsRider { get; set; }
        public List<rental> Rentals { get; set; }
    }
}

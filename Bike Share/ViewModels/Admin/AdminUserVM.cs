using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeShare.ViewModels.Admin
{
    public class AdminUserVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? LastRegistered { get; set; }
        public bool IsArchived { get; set; }
        public int? RegistrationPDFNumber { get; set;  }
    }
}

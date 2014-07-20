using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class bikeUser
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int bikeUserId { get; set; }
        
        public DateTime lastRegistered { get; set; }
        
        [Required]
        public string userName { get; set; }
        
        [Required]
        public Boolean hasBike { get; set; }
        
        public virtual Bike bike { get; set; }
        
        public virtual ICollection<CheckOut> checkOuts { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Phone]
        public string phoneNumber { get; set; }

        [Required]
        public bool isArchived { get; set; }

        public virtual ICollection<MaintenanceEvent> maintenanceEvents { get; set; }

        public virtual ICollection<MaintenanceUpdate> maintenanceUpdates { get; set; }

        public virtual ICollection<Charge> charges { get; set; }

        [Required]
        public bool canMaintainBikes { get; set; }

        [Required]
        public bool canBorrowBikes { get; set; }

        [Required]
        public bool canAdministerSite { get; set; }

        [Required]
        public bool canCheckOutBikes { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

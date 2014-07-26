using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class CheckOut
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int checkOutId { get; set; }
        
        public DateTime timeOut { get; set; }

        public DateTime timeIn { get; set; }
        
        public virtual bikeUser checkOutPerson { get; set; }
        
        public virtual bikeUser user { get; set; }
        
        public virtual Bike bike { get; set; }

        public virtual BikeRack rackCheckedOut { get; set;}

        public virtual BikeRack rackCheckedIn { get; set; }

        [Required]
        public bool isResolved { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

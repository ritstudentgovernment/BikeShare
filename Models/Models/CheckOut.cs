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
        
        public bikeUser checkOutPerson { get; set; }
        
        public bikeUser user { get; set; }
        
        public Bike bike { get; set; }

        public BikeRack rackCheckedOut { get; set;}

        public BikeRack rackCheckedIn { get; set; }

        [Required]
        public bool isResolved { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

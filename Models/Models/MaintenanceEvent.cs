using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class MaintenanceEvent
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int MaintenanceEventId { get; set; }
        
        [Required]
        public virtual Bike bikeAffected { get; set; }
        
        [Required]
        public string title { get; set; }
        
        public string details { get; set; }
        
        [Required]
        public virtual bikeUser staffPerson { get; set; }
        
        [Required]
        public DateTime timeAdded { get; set; }
        
        [Required]
        public virtual Workshop workshop { get; set; }
        
        public virtual ICollection<MaintenanceUpdate> updates { get; set; }
        
        [Required]
        public bool resolved { get; set; }
        
        public DateTime timeResolved { get; set; }

        [Required]
        public bool disableBike { get; set; }

        [Required]
        public bool isArchived { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

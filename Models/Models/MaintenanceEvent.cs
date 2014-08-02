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
        [Display(Name = "Bike")]
        public virtual Bike bikeAffected { get; set; }
        
        [Required]
        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Description")]
        public string details { get; set; }
        
        [Required]
        [Display(Name = "Maintained by")]
        public virtual bikeUser staffPerson { get; set; }
        
        [Required]
        [Display(Name = "Date Opened")]
        public DateTime timeAdded { get; set; }
        
        [Required]
        [Display(Name = "Workshop")]
        public virtual Workshop workshop { get; set; }
        
        public virtual ICollection<MaintenanceUpdate> updates { get; set; }
        
        [Required]
        [Display(Name = "Open?")]
        public bool resolved { get; set; }

        [Display(Name = "Date Closed")]
        public DateTime timeResolved { get; set; }

        [Required]
        [Display(Name = "Disable Bike?")]
        public bool disableBike { get; set; }

        [Required]
        public bool isArchived { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

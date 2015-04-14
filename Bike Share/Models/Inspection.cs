using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class Inspection
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int inspectionId { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime datePerformed { get; set; }

        [Required]
        [Display(Name = "Inspected by")]
        public virtual bikeUser inspector { get; set; }

        [Required]
        public virtual Bike bike { get; set; }

        [Required]
        [Display(Name = "Workshop")]
        public virtual Workshop placeInspected { get; set; }

        public virtual ICollection<MaintenanceEvent> associatedMaintenance { get; set; }

        [Required]
        [Display(Name = "Pass?")]
        public Boolean isPassed { get; set; }

        [Display(Name = "Comment")]
        public string comment { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

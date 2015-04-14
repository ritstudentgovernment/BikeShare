using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class Bike
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int bikeId { get; set; }

        [Required]
        [Display(Name = "Bike Number")]
        public int bikeNumber { get; set; }

        [Required]
        [Display(Name = "Bike Name")]
        public string bikeName { get; set; }

        [Display(Name = "Last Checked Out")]
        public DateTime lastCheckedOut { get; set; }

        public virtual ICollection<CheckOut> checkOuts { get; set; }

        public virtual BikeRack bikeRack { get; set; }

        [Required]
        public bool isArchived { get; set; }

        public virtual ICollection<Tracer> events { get; set; }

        public DateTime lastPassedInspection { get; set; }
    }
}
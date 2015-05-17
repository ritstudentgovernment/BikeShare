using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class Bike
    {
        public Bike()
        {
            
        }

        public Bike(int number)
        {
            bikeNumber = number;
            bikeName = "Bike " + bikeNumber;
            lastCheckedOut = new DateTime(2000, 01, 01);
            onInspectionHold = false;
            onMaintenanceHold = false;
        }
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

        public int? bikeRackId { get; set; }

        [Required]
        public bool isArchived { get; set; }

        public bool onInspectionHold { get; set; }

        public bool onMaintenanceHold { get; set; }

        public bool isAvailable()
        {
            return !onInspectionHold && !onMaintenanceHold;
        }

    }
}
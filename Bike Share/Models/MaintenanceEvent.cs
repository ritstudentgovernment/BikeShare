using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class MaintenanceEvent
    {
        public MaintenanceEvent()
        {
            isArchived = false;
            timeAdded = DateTime.Now;
        }
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int MaintenanceEventId { get; set; }

        [Required]
        [Display(Name = "Bike")]
        public int bikeId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Description")]
        public string details { get; set; }

        [Required]
        [Display(Name = "Submitted by")]
        public int submittedById { get; set; }

        public int? maintainedById { get; set; }

        [Required]
        [Display(Name = "Date Opened")]
        public DateTime timeAdded { get; set; }

        [Display(Name = "Date Closed")]
        public DateTime? timeResolved { get; set; }

        [Required]
        [Display(Name = "Disable Bike?")]
        public bool disableBike { get; set; }

        [Required]
        public bool isArchived { get; set; }
    }
}
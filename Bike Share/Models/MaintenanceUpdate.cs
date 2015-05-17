using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class MaintenanceUpdate
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int MaintenanceUpdateId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Body")]
        public string body { get; set; }

        [Display(Name = "Posted")]
        public DateTime timePosted { get; set; }

        [Required]
        [Display(Name = "Poster")]
        public int postedById { get; set; }

        public int associatedEventId { get; set; }

        public int bikeId { get; set; }

        public bool isCommentOnBike { get; set; }
    }
}
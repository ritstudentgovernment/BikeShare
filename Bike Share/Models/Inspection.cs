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
        public int inspectorId { get; set; }

        [Required]
        public int bikeId { get; set; }

        [Required]
        [Display(Name = "Workshop")]
        public int placeInspectedId { get; set; }

        [Required]
        [Display(Name = "Pass?")]
        public Boolean isPassed { get; set; }

        [Display(Name = "Comment")]
        public string comment { get; set; }
    }
}
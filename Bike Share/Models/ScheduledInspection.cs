using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class ScheduledInspection
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name="Schedule Name")]
        public string name { get; set; }
        public DayOfWeek day { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        [Display(Name="Affected Bikes")]
        public string affectedBikes { get; set; }
        [Display(Name="Hour (0-23):")]
        public short hour { get; set; }
        public DateTime lastRun { get; set; }
    }
}

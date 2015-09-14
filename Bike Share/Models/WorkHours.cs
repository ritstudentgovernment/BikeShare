using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class WorkHour
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [Required]
        public int workHourId { get; set; }

        public string comment { get; set; }

        public int userid { get; set; }

        public DateTime timeStart { get; set; }

        public DateTime timeEnd { get; set; }
    }
}
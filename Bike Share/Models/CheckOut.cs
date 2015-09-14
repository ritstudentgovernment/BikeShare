using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class CheckOut
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int checkOutId { get; set; }

        public DateTime timeOut { get; set; }

        public DateTime? timeIn { get; set; }

        public int checkOutPerson { get; set; }

        public int rider { get; set; }

        public int bike { get; set; }

        public int rackCheckedOut { get; set; }

        public int rackCheckedIn { get; set; }

        [Required]
        public bool isResolved { get; set; }
    }
}
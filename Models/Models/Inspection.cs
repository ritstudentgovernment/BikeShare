using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public DateTime datePerformed { get; set; }

        [Required]
        public virtual bikeUser inspector { get; set; }

        [Required]
        public virtual Bike bike { get; set; }

        [Required]
        public virtual Workshop placeInspected { get; set; }

        public virtual ICollection<MaintenanceEvent> associatedMaintenance { get; set; }

        [Required]
        public Boolean isPassed { get; set; }

        public string comment { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

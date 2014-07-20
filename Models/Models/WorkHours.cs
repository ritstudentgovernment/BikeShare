using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public virtual Bike bike { get; set; }

        public virtual BikeRack rack { get; set; }

        public virtual Workshop shop { get; set; }

        public virtual MaintenanceEvent maint { get; set; }

        public virtual bikeUser user { get; set; }

        public DateTime timeStart { get; set; }

        public DateTime timeEnd { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

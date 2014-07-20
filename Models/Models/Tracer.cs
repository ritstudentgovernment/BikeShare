using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BikeShare.Models
{
    public class Tracer
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int tracerId { get; set; }
        public string comment { get; set; }
        public DateTime time { get; set; }
        public virtual Bike bike { get; set; }
        public virtual BikeRack rack { get; set; }
        public virtual bikeUser user { get; set; }
        public virtual Workshop shop { get; set; }
        public virtual MaintenanceEvent maint { get; set; }
        public virtual MaintenanceUpdate update { get; set; }
        public virtual WorkHour workHour { get; set; }
        public virtual CheckOut checkOut { get; set; }
        public virtual Inspection inspection { get; set; }
        public virtual Charge charge { get; set; }
    }
}

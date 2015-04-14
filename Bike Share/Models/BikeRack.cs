using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class BikeRack
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [Required]
        public int bikeRackId { get; set; }

        [Required]
        [Display(Name = "Rack Name")]
        public string name { get; set; }

        [Range(-85.0, 85.0)]
        [Required]
        public float GPSCoordX { get; set; }

        [Range(-180, 180)]
        [Required]
        public float GPSCoordY { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }

        public virtual ICollection<Bike> bikes { get; set; }

        public virtual ICollection<CheckOut> checkOuts { get; set; }

        [Required]
        public bool isArchived { get; set; }

        public virtual ICollection<Tracer> events { get; set; }

        public virtual ICollection<Hour> hours { get; set; }
    }
}
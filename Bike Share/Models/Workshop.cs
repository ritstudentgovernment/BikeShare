using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class Workshop
    {
        public Workshop()
        {
            isArchived = false;
        }

        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int workshopId { get; set; }

        [Required]
        [Display(Name = "Workshop Name")]
        public string Name { get; set; }

        [Required]
        [Range(-85.0, 85.0)]
        public float GPSCoordX { get; set; }

        [Range(-180, 180)]
        [Required]
        public float GPSCoordY { get; set; }

        [Required]
        public bool isArchived { get; set; }

        public string hours { get; set; }
    }
}
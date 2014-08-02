using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public virtual bikeUser postedBy { get; set; }

        public virtual MaintenanceEvent associatedEvent { get; set; }

        public virtual Bike bike { get; set; }

        public bool isCommentOnBike { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

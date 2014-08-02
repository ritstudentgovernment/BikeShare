using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class Charge
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int chargeId { get; set; }

        [Required]
        [Display(Name = "Charge Title")]
        public string title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Date Assessed")]
        public DateTime dateAssesed { get; set; }

        [Display(Name = "Date Resolved")]
        public DateTime dateResolved { get; set; }

        [Required]
        public Boolean isResolved { get; set; }

        public int notificationsCounter { get; set; }

        [Required]
        public virtual bikeUser user { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public decimal amountCharged { get; set; }

        [Display(Name = "Paid")]
        public decimal amountPaid { get; set; }

        public virtual ICollection<Tracer> events { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{

    public class MailSub
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int subId { get; set; }

        [Required]
        public virtual bikeUser subscriber { get; set; }

        [EmailAddress]
        public string email { get; set; }

        public virtual Bike bike { get; set; }

        public virtual BikeRack rack { get; set; }

        public virtual Workshop workshop { get; set; }
    }
}
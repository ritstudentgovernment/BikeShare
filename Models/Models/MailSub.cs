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
        public bikeUser subscriber { get; set; }

        [EmailAddress]
        public string email { get; set; }

        public Bike bike { get; set; }

        public BikeRack rack { get; set; }

        public Workshop workshop { get; set; }
    }
}
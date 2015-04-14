using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public class Hour
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int hourId { get; set; }
        [Display(Name = "Starting Day of Week")]
        public string dayStart { get; set; }
        [Display(Name = "Ending Day of Week")]
        public string dayEnd { get; set; }
        [Display(Name = "Starting Hour")]
        public int hourStart { get; set; }
        [Display(Name = "Ending Hour")]
        public int hourEnd { get; set; }
        [Display(Name = "Starting Minute")]
        public int minuteStart { get; set; }
        [Display(Name = "Ending Minute")]
        public int minuteEnd { get; set; }
        [Display(Name = "Open?")]
        public bool isOpen { get; set; }
        [Display(Name = "Comment")]
        public string comment { get; set; }
        public virtual Workshop shop { get; set;  }
        public virtual BikeRack rack { get; set; }
    }
}

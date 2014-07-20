using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BikeShare.Models
{
    public class Hour
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int hourId { get; set; }
        public string dayStart { get; set; }
        public string dayEnd { get; set; }
        public int hourStart { get; set; }
        public int hourEnd { get; set; }
        public int minuteStart { get; set; }
        public int minuteEnd { get; set; }
        public bool isOpen { get; set; }
        public string comment { get; set; }
        public virtual Workshop shop { get; set;  }
        public virtual BikeRack rack { get; set; }
    }
}

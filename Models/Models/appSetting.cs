using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BikeShare.Models
{
    public class appSetting
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int settingId { get; set; }
        public string appName { get; set; }
        public string expectedEmail { get; set; }
        public int maxRentDays { get; set; }
        public int DaysBetweenInspections { get; set; }
        public int daysBetweenRegistrations { get; set; }
        public int overdueBikeMailingIntervalHours { get; set; }
    }
}

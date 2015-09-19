using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BikeShare.ViewModels.Admin
{
    public class AdminBikeVM
    {
        [Display(Name="Archived", Description="Archived bikes are not available throughout the application. Best used for bikes that have been discarded.")]
        public bool IsArchived { get; set; }

        [Display(Name="Last Borrowed", Description="The date of the last time the bike was borrowed.")]
        public DateTime? LastBorrowed { get; set; }

        [Display(Name = "Availability", Description = "The current availability of the bike.")]
        public bool IsAvailable { get; set; }

        [Display(Description="The name of the bike. Can be used for sponsored or branded bikes.")]
        public string Name { get; set; }

        [Display(Description="The number of the bike as it is displayed on the bike.")]
        public int Number { get; set; }

        [Display(Description="The unique identifier of the bike within the website.")]
        public int Id { get; set; }

        [Display(Description="The username of the last person to borrow the bike.")]
        public string LastCheckedOutTo { get; set; }

        public string Notes { get; set; }
    }
}

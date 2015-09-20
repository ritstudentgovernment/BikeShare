using System.ComponentModel.DataAnnotations;

namespace BikeShare.ViewModels
{
    public class newBikeViewModel
    {
        public int bikeRackId { get; set; }

        [Display(Name = "Bike Name", Description = "Used for giving a text name to a bike. Example uses include giving each bike a fun name, or allowing donors to name a bike.")]
        public string bikeName { get; set; }

        [Display(Name = "Bike Number", Description = "Bike number.")]
        public int bikeNumber { get; set; }
    }
}
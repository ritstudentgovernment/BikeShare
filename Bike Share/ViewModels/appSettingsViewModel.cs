using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BikeShare.ViewModels
{
    public class appSettingsViewModel
    {
        [Display(Name="Application Name", Description="The name of the application as it will appear in the header and emails.")]
        public string appName { get; set; }
        [Display(Name = "Maximum bike rental length (days)", Description = "The number of days that a bike may be checked out. This is used to automatically mark bikes as past due.")]
        public int rentPeriodMaxDays { get; set; }
        [Display(Name = "Maximum user balance", Description = "Once a user reaches this balance, they will not be able to check out any more bikes.")]
        public int maxBalance { get; set; }
    }
}
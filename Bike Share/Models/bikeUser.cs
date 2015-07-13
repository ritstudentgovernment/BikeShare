using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShare.Models
{
    public enum privilege { rider, admin, mechanic, cashier}

    public class bikeUser
    {
        [Required]
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int bikeUserId { get; set; }

        public DateTime lastRegistered { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string userName { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string phoneNumber { get; set; }

        [Required]
        public bool isArchived { get; set; }

        [Required]
        [Display(Name = "Mechanic Privileges")]
        public bool canMaintainBikes { get; set; }

        [Required]
        [Display(Name = "Rider Privileges")]
        public bool canBorrowBikes { get; set; }

        [Required]
        [Display(Name = "Admin Privileges")]
        public bool canAdministerSite { get; set; }

        [Required]
        [Display(Name = "Cashier Privileges")]
        public bool canCheckOutBikes { get; set; }

        public int registrationPDFNumber { get; set; }
    }
}
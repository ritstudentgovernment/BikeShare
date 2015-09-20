using System;
using System.ComponentModel.DataAnnotations;

namespace BikeShare.ViewModels
{
    public class bikeUserPermissionViewModel
    {
        [Display(Name = "User Name", Description = "The institutional id for the user that is used for login purposes.")]
        public string userName { get; set; }

        public int userId { get; set; }

        [Display(Name = "Email", Description = "Used for contacting the user automatically with reminder emails.")]
        public string email { get; set; }

        [Display(Name = "Phone", Description = "Phone number stored for record-keeping purposes.")]
        public string phone { get; set; }

        [Display(Name = "Cashier Privileges", Description = "Users with this privilege may check bikes out and assess charges.")]
        public Boolean canCheckOutBikes { get; set; }

        [Display(Name = "Rider Privileges", Description = "Users with this privilege may borrow bikes. Note: User must also have registered in order to rent bikes.")]
        public Boolean canBorrowBikes { get; set; }

        [Display(Name = "Mechanic Privileges", Description = "Users with this privilege may enter maintenance and inspections for bikes.")]
        public Boolean canMaintainBikes { get; set; }

        [Display(Name = "Administrator Privileges", Description = "Users with this privilege may enter the administration portal.")]
        public Boolean canManageApp { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace BikeShare.Models
{
    public class Login
    {
        [Required]
        [Display(Name = "User Name", Prompt = "User name required. Do not include the @.com part of the name.")]
        public String UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Password required.")]
        public String Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
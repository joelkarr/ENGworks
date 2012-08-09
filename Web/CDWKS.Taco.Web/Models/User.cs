using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDWKS.BXC.Taco.Web.Models
{
    public class User
    {
        [Required]
        [Email]
        public string Email { get; set; }
          [Required]
          [DataType(DataType.Password)]
        public string Password { get; set; }
          [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
          [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
          [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
          [Required]
        public string Company { get; set; }
          [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

          public string Message { get; set; }

    }
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CDWKS.BXC.Taco.Web.Models
{
    public class IndexViewModel : BaseViewModel
    {
        public UserInput UserInput { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool LoginFail { get; set; }
        public bool Register { get; set; }
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
        public IndexViewModel()
        {
            UserInput = new UserInput();
            
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace LiteForum.ViewModels
{
    public class LoginVModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The username must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
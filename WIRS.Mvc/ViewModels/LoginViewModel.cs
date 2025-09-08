using System.ComponentModel.DataAnnotations;

namespace WIRS.Mvc.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User ID is required")]
        [Display(Name = "User ID")]
        [StringLength(8)]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [StringLength(12)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? PageId { get; set; }
    }
}
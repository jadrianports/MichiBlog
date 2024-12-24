using System.ComponentModel.DataAnnotations;

namespace MichiBlog.WebApp.ViewModels
{
    public class ResetPassword
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string? NewPassword { get; set;  }
        [Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
    }
}

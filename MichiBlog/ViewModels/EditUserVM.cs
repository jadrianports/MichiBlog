using System.ComponentModel.DataAnnotations;

namespace MichiBlog.WebApp.ViewModels
{
    public class EditUserVM
    {
        public string? Id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public bool IsAdmin { get; set; } = false;
    }
}

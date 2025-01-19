using System.ComponentModel.DataAnnotations;

namespace MichiBlog.WebApp.ViewModels
{
    public class ContactMessageVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        [Required]
        public string Message { get; set; }
    }
}

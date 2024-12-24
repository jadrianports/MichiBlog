using Microsoft.AspNetCore.Identity;

namespace MichiBlog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        //relation
        public List<CreatePostVM> Posts { get; set; }
    }
}

using MichiBlog.Models;

namespace MichiBlog.WebApp.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public bool Liked { get; set; }
        public bool Disliked { get; set; }  
        public int Count { get; set; } = 0;

        // Relationships

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; } = null!;
        public int PostId { get; set; }
        public  Post Post { get; set; } = null!;
    }
}

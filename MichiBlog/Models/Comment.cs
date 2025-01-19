using MichiBlog.Models;

namespace MichiBlog.WebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Relationships
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public int? ParentCommentId { get; set; }  // For nested replies
        public  Comment? ParentComment { get; set; }  // Navigation property
        public ICollection<Comment>? Replies { get; set; }  // Replies to this comment
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}

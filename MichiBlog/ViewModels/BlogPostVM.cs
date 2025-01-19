using MichiBlog.WebApp.Models;

namespace MichiBlog.WebApp.ViewModels
{
    public class BlogPostVM
    {
        //List properties of a post to be displayed in a list
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Author { get; set; }
        
        public ICollection<Comment> Comments { get; set; }
        public List<Reaction> Reactions { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public bool CurrentLiked { get; set; }
        public bool CurrentDisliked { get; set; }
        public bool UserHasReacted { get; set; } // To track if the current user has reacted

    }
}

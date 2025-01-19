using MichiBlog.WebApp.Models;

namespace MichiBlog.WebApp.ViewModels
{
    public class HomeVM
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<Post>? Posts { get; set; }
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        //Make a model for the amount of comments and reactions on posts

    }
}

using MichiBlog.Models;
using System.Web.Mvc;

namespace MichiBlog.WebApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        [AllowHtml] 
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? ThumbnailUrl { get; set; }

        //relationships
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}

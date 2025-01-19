namespace MichiBlog.WebApp.ViewModels
{
    public class PageVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public string? Slug { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}

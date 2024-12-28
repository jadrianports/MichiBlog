namespace MichiBlog.WebApp.ViewModels
{
    public class PostVM
    {
        //List properties of a post to be displayed in a list
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Author { get; set; }
    }
}

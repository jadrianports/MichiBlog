﻿namespace MichiBlog.WebApp.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public string? Title {  get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
    }
}

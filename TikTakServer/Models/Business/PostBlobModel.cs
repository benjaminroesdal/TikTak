﻿namespace TikTakServer.Models.Business
{
    public class PostBlobModel
    {
        public List<TagModel> Tags { get; set; }
        public IFormFile File { get; set; }
    }
}

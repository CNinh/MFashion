using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class BlogResponse
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
    }

    public class BlogDetailResponse
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PublishedBy { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<BlogTagResponse> Tags { get; set; }
        public List<CommentResponse> Comments { get; set; }
    }

    public class CommentResponse
    {
        public int Id { get; set; }
        public string? Avatar { get; set; }
        public string FullName { get; set; }
        public string Content { get; set; }
    }

    public class BlogTagResponse
    {
        public int Id { get; set; }
        public string TagName { get; set; }
    }
}

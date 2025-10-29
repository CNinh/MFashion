using DataAccessObject.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelRequest
{
    public class BlogRequest
    {
        [DefaultValue(null)]
        public string? Title { get; set; }

        [DefaultValue(null)]
        public int? PublishedBy { get; set; }

        [DefaultValue(null)]
        public int? CategoryId {  get; set; }

        [DefaultValue(1)]
        public int PageIndex { get; set; } = 1;

        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;

        [DefaultValue("")]
        public string SortBy { get; set; } = "";

        [DefaultValue(false)]
        public bool Decending { get; set; } = false;
    }

    public class CreateBlogRequest
    {
        [Required(ErrorMessage = "Blog title is required!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Blog content is required!")]
        public string Content { get; set; }

        [Required(ErrorMessage = "CategoryId is required!")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Blog tags are required!")]
        public List<int> TagId { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class UpdateBlogRequest
    {
        [Required(ErrorMessage = "Blog title is required!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Blog content is required!")]
        public string Content { get; set; }

        [Required(ErrorMessage = "CategoryId is required!")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Blog tags are required!")]
        public List<int> TagId { get; set; }
        public List<int>? KeepImageIds { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class CommmentRequest
    {
        [Required(ErrorMessage = "Comment is required!")]
        public string Content { get; set; }
    }
}

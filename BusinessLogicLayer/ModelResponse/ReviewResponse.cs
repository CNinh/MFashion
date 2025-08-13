using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
        public DateTime CreateAt { get; set; }
        public List<string> Images { get; set; }
    }

    public class ProductReviewResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Rate { set; get; }
        public string Comment { get; set; }
        public DateTime CreatAt { get; set; }
        public List<string> Images { get; set; }
    }

    public class UpdateReviewResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Rate { set; get; }
        public string Comment { get; set; }
        public DateTime? UpdateAt {  get; set; }
        public bool IsUpdated { get; set; }
        public List<string> Images { get; set; }
    }
}

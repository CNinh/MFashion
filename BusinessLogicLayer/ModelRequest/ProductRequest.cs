using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelRequest
{
    public class ProductRequest
    {
    }

    public class ProductListRequest
    {
        public int? UserId { get; set; }
        public string? ProductName { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "";
        public bool Descending { get; set; } = false;
    }

    public class CreateProductRequest
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? SKU { get; set; }
        public ProductStatus Status { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public ICollection<TagResponse> Tags { get; set; } = new List<TagResponse>();
    }

    public class UpdateProductRequest
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? SKU { get; set; }
        public ProductStatus Status { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public ICollection<TagResponse> Tags { get; set; } = new List<TagResponse>();
    }
}

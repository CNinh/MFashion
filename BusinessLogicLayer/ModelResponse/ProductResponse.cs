using DataAccessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Rate { get; set; }
        public int TotalRate { get; set; }
        public int TotalSold { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public Product.ProductStatus Status { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<ColorResponse> Colors { get; set; } = new List<ColorResponse>();
        public List<DeliveryResponse> Deliveries { get; set; } = new List<DeliveryResponse>();
        public List<MaterialResponse> Materials { get; set; } = new List<MaterialResponse>();
        public List<SizeResponse> Sizes { get; set; } = new List<SizeResponse>();
        public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
    }

    public class ProductListResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public List<ColorResponse> Colors { get; set; } = new List<ColorResponse>();
    }

    public class CreateProductResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? SKU { get; set; }
        public Product.ProductStatus Status { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreateAt { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
    }

    public class UpdateProductResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? SKU { get; set; }
        public Product.ProductStatus Status { get; set; }
        public int CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }

    public class ColorResponse
    {
        public int Id { get; set; }
        public string ThemeColor { get; set; }
    }

    public class MaterialResponse
    {
        public int Id { get; set; }
        public string MaterialType { get; set; }
    }

    public class DeliveryResponse
    {
        public int Id { get; set; }
        public string DeliveryType { get; set; }
    }

    public class SizeResponse
    {
        public int Id { get; set; }
        public string ProductSize { get; set; }
    }

    public class TagResponse
    {
        public int Id { get; set; }
        public string TagName { get; set; }
    }
}

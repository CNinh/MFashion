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
    public class ProductRequest
    {
    }

    public class ProductListRequest
    {
        [DefaultValue(null)]
        public string? ProductName { get; set; }

        [DefaultValue(null)]
        public int? MinPrice { get; set; }

        [DefaultValue(null)]
        public int? MaxPrice { get; set; }

        [DefaultValue(1)]
        public int PageIndex { get; set; } = 1;

        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;

        [DefaultValue("")]
        public string SortBy { get; set; } = "";

        [DefaultValue(false)]
        public bool Descending { get; set; } = false;
    }

    public class GetProductByVendorRequest
    {
        [Required]
        public string Slug { get; set; }

        [DefaultValue(null)]
        public string? ProductName { get; set; }

        [DefaultValue(null)]
        public int? MinPrice { get; set; }

        [DefaultValue(null)]
        public int? MaxPrice { get; set; }

        [DefaultValue(1)]
        public int PageIndex { get; set; } = 1;

        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;

        [DefaultValue("")]
        public string SortBy { get; set; } = "";

        [DefaultValue(false)]
        public bool Descending { get; set; } = false;
    }

    public class CreateProductRequest
    {
        [Required(ErrorMessage = "Product name is required!")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Quantity is required!")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity value cannot be negative.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Product price is required!")]
        [Range(1, (double)decimal.MaxValue, ErrorMessage = "Product price value must be positive.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description of product is required!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status of product is required!")]
        public Product.ProductStatus Status { get; set; }

        [Required(ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Product images are required!")]
        public List<IFormFile> Images { get; set; }
        public List<int> ColorId { get; set; } = new List<int>();
        public List<int> SizeId { get; set; } = new List<int>();
        public List<int> TagId { get; set; } = new List<int>();
    }

    public class UpdateProductRequest
    {
        [Required(ErrorMessage = "Product name is required!")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Quantity is required!")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity value cannot be negative.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Product price is required!")]
        [Range(1, (double)decimal.MaxValue, ErrorMessage = "Product price value must be positive.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description of product is required!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status of product is required!")]
        public Product.ProductStatus Status { get; set; }

        [Required(ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }

        public List<int>? KeepImageIds { get; set; }

        [Required(ErrorMessage = "Product images are required!")]
        public List<IFormFile> Images { get; set; }
        public List<int> ColorId { get; set; } = new List<int>();
        public List<int> SizeId { get; set; } = new List<int>();
        public List<int> TagId { get; set; } = new List<int>();
    }
}

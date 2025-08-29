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
    public class CartRequest
    {
        public int TotalItem { get; set; }
        public decimal TotalPrice { get; set; }
        public int AccountId { get; set; }
    }

    public class CartItemRequest
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Item in cart quantity must exists.")]
        public int Quantity { get; set; }
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Unit price cannot be negative.")]
        public decimal UnitPrice { get; set; }
    }

    public class AddToCartRequest
    {
        [Required(ErrorMessage = "Product is required!")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must at least 1")]
        [DefaultValue(1)]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Color is required!")]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Delivery is required!")]
        public int DeliveryId { get; set; }

        [Required(ErrorMessage = "Material is required!")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Size is required!")]
        public int SizeId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }

    public class UpdateQuantityRequest
    {
        [Required(ErrorMessage = "Item is required!")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must at least 1")]
        [DefaultValue(1)]
        public int Quantity { get; set; }
    }

    public class RemoveProductRequest
    {
        [Required(ErrorMessage = "Product is required!")]
        public int ItemId { get; set; }
    }
}

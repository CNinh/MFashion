using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Model
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? SKU { get; set; }
        public enum ProductStatus
        {
            OnSale,
            InStock,
            OutOfStock,
            OnBackOrder
        }
        public ProductStatus Status { get; set; }
        public DateTime CreateAt { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; }

        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public int DeliveryId { get; set; }
        public Delivery Delivery { get; set; }

        public int CategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductDesign> ProductDesigns { get; set; } = new List<ProductDesign>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Model
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string ProductName { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Note { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }

        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; }

        public virtual ICollection<Design> Designs { get; set; } = new List<Design>();
    }
}

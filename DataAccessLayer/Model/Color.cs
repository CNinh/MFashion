using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Model
{
    public class Color
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(9)")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$")]
        public string PrimaryHex { get; set; }
        public string PrimaryColor { get; set; }

        [Column(TypeName = "nvarchar(9)")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$")]
        public string? SecondaryHex { get; set; }
        public string? SecondaryColor { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}

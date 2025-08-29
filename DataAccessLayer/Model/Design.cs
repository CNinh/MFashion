using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Model
{
    public class Design
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileUrl { get; set; }
        public string PublicId { get; set; }
        public string ResourceType { get; set; }

        public int? CartItemId { get; set; }
        public CartItem? CartItem { get; set; }

        public int? OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class CartResponse
    {
        public int Id { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalPrice { get; set; }
        public int AccountId { get; set; }
        public int? VoucherId { get; set; }
        public ICollection<CartItemResponse> CartItems { get; set; }
    }

    public class CartItemResponse
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Delivery { get; set; }
        public string Material { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

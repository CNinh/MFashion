using DataAccessObject.Migrations;
using DataAccessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int TotalItem {  get; set; }
        public decimal TotalPrice { get; set; }
        public Order.OrderStatus Status { get; set; }
    }

    public class DetailResponse
    {
        public string OrderCode { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalPrice { get; set; }
        public Order.OrderStatus Status { get; set; }
        public ICollection<OrderDetailResponse> OrderDetails { get; set; }
        public int AccountId { get; set; }
        public AddressResponse Address { get; set; }
    }

    public class OrderDetailResponse
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public List<string> Design { get; set; }
        public string Material { get; set; }
        public string Size { get; set; }
        public string? Note { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class AddressResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CompanyName { get; set; }
        public string Address { get; set; }
        public string? SubAddress { get; set; }
        //public string Country { get; set; }
        //public string? State { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        //public string Email { get; set; }
    }
}

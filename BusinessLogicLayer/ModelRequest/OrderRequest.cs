using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessObject.Model;

namespace BusinessLogicLayer.ModelRequest
{
    public class OrderRequest
    {
        [DefaultValue(null)]
        public string? OrderCode { get; set; }

        [DefaultValue(null)]
        public Order.OrderStatus? Status { get; set; }

        [DefaultValue(null)]
        public int? MinPrice { get; set; }

        [DefaultValue(null)]
        public int? MaxPrice { get; set; }

        [DefaultValue(20)]
        public int PageIndex { get; set; }

        [DefaultValue(1)]
        public int PageSize { get; set; }

        [DefaultValue("")]
        public string SortBy { get; set; } = "";

        [DefaultValue(false)]
        public bool Descending { get; set; } = false;
    }
    public class OrderDetailRequest
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class AddressRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CompanyName { get; set; }
        public string Address { get; set; }
        public string? SubAddress { get; set; }
        //public string Country { get; set; }
        //public string? State { get; set; }
        public string? Province { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        //public string Email { get; set; }
    }

    public class CreateOrderRequest : AddressRequest
    {
        public int DeliveryId { get; set; }
    }
}

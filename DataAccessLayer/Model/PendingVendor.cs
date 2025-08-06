using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Model
{
    public class PendingVendor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShopName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreateAt { get; set; }
    }
}

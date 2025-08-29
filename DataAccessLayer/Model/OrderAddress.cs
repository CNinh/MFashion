using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Model
{
    public class OrderAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CompanyName { get; set; }
        public string Address { get; set; }
        public string? SubAddress { get; set; }
        public string Country { get; set; }
        public string? State { get; set; }
        public string? Province { get; set; }
        public string City { get; set; }

        [Column(TypeName = "nvarchar(12)")]
        public string PostCode { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}

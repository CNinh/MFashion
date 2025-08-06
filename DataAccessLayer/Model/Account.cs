using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Model
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? Username { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? ShopName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? Slug { get; set; }
        public bool IsDisable { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorToken { get; set; }
        public DateTime? TwoFactorTokenExpiry { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public Cart Cart { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

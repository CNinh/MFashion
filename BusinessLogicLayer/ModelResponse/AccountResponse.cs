using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public string? Avatar { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ShopName { get; set; }
        public bool IsDisalbe { get; set; }
    }

    public class AccountListResponse
    {
        public int Id { get; set; }
        public string? Avatar {  get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Slug { get; set; }
        public bool IsDisable { get; set; }
    }

    public class UpdateProfileResponse
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Slug { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}

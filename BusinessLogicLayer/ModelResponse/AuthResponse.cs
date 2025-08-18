using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public AuthResponse()
        {
            Errors = new List<string>();
            Token = string.Empty;
        }
    }

    public class LoginResponse
    {
        public string? Token { get; set; }
        public int? RoleId { get; set; }
        public string? Role { get; set; }
        public string? Slug { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public bool RequiredTwoFactor { get; set; } = false;
    }
}

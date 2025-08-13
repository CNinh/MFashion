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
        public required string Token { get; set; }
        public int RoleId { get; set; }
        public required string Role { get; set; }
        public bool RequiredTwoFactor { get; set; } = false;
    }
}

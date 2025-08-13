using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IEmailService
    {
        string GenerateVerificationEmail(string email, TimeSpan expireIn, string role);
        Task SendEmailAsync(string toEmail, string subject, string body);
        ClaimsPrincipal? ValidateToken(string token);
    }
}

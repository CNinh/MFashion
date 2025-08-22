using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using BusinessLogicLayer.Utilities;
using DataAccessObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Account> _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly ICartService _cartService;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<Account>();
            _emailService = emailService;
            _cartService = cartService;
        }

        public async Task<BaseResponse> RegisterCustomerAsync(RegisterRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var existingAccount = await _unitOfWork.AccountRepository.Queryable()
                                        .Where(a => a.Email == request.Email)
                                        .FirstOrDefaultAsync();

                if (existingAccount != null)
                {
                    response.Message = "Email already registered!";
                    return response;
                }

                var pending = new PendingAccount
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreateAt = TimeHelper.VietnamTimeZone()
                };

                await _unitOfWork.PendingAccountRepository.InsertAsync(pending);
                await _unitOfWork.CommitAsync();

                var token = _emailService.GenerateVerificationEmail(request.Email, TimeSpan.FromMinutes(10), "customer");

                // Create register password link
                var link = $"http://localhost:3000/setup-password?token={token}&type=customer";

                // Send email
                var subject = "Register Account";
                var body = $"Click here to finish your registration: <a href='{link}'>Set Your Password</a>";

                await _emailService.SendEmailAsync(request.Email, subject, body);

                response.Success = true;
                response.Message = "Registration email has been sent, please check your email.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error has occured, please try again later!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> RegisterVendorAsync(RegisterVendorRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var existingAccount = await _unitOfWork.AccountRepository.Queryable()
                                        .Where(a => a.Email == request.Email)
                                        .FirstOrDefaultAsync();

                if (existingAccount != null)
                {
                    response.Message = "Email already registered!";
                    return response;
                }

                var existingShop = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.ShopName == request.ShopName)
                                    .FirstOrDefaultAsync();

                if (existingShop != null)
                {
                    response.Message = "Shop name has been used!";
                    return response;
                }

                var existingPhone = await _unitOfWork.AccountRepository.Queryable()
                                        .Where(a => a.PhoneNumber == request.PhoneNumber)
                                        .FirstOrDefaultAsync();

                if (existingPhone != null)
                {
                    response.Message = "Phone number has been taken by another account!";
                    return response;
                }

                var existingPending = await _unitOfWork.PendingAccountRepository.Queryable()
                                            .Where(pv => pv.Email == request.Email)
                                            .ToListAsync();

                if (existingPending.Any())
                {
                    _unitOfWork.PendingAccountRepository.RemoveRange(existingPending);
                }

                var pending = new PendingAccount
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    ShopName = request.ShopName,
                    PhoneNumber = request.PhoneNumber,
                    CreateAt = TimeHelper.VietnamTimeZone()
                };

                await _unitOfWork.PendingAccountRepository.InsertAsync(pending);
                await _unitOfWork.CommitAsync();

                var token = _emailService.GenerateVerificationEmail(request.Email, TimeSpan.FromMinutes(10), "vendor");

                // Create register password link
                var link = $"http://localhost:3000/setup-password?token={token}&type=vendor";

                // Send email
                var subject = "Register Account";
                var body = $"Click here to finish your registration: <a href='{link}'>Set Your Password</a>";

                await _emailService.SendEmailAsync(request.Email, subject, body);

                response.Success = true;
                response.Message = "Registration email has been sent, please check your email.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error has occured, please try again later!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> SetupPasswordAsync(SetupPasswordRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var principal = _emailService.ValidateToken(request.Token);

                if (principal == null)
                {
                    response.Message = "Invalid or Expired token!";
                    return response;
                }

                var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var type = principal.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
                var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(email) || type != "register")
                {
                    response.Message = "Invalid token!";
                    return response;
                }

                var existingAccount = await _unitOfWork.AccountRepository.Queryable()
                                            .Where(a => a.Email == email)
                                            .FirstOrDefaultAsync();

                if (existingAccount != null)
                {
                    response.Message = "Account already exist!";
                    return response;
                }

                var pending = await _unitOfWork.PendingAccountRepository.Queryable()
                                    .Where(pv => pv.Email == email)
                                    .FirstOrDefaultAsync();

                if (pending == null)
                {
                    response.Message = "Registration data not found or expired!";
                    return response;
                }

                Account account;

                if (role == "customer")
                {
                    account = new Account
                    {
                        Email = email,
                        FirstName = pending.FirstName,
                        LastName = pending.LastName,
                        RoleId = 3,
                        IsDisable = false,
                        TwoFactorEnabled = false,
                        Slug = await GenerateSlugAsync($"{email}", null, $"{pending.LastName} {pending.FirstName}")
                    };

                    _unitOfWork.PendingAccountRepository.RemoveEntity(pending);
                }
                else if (role == "vendor")
                {
                    account = new Account
                    {
                        Email = email,
                        RoleId = 2,
                        FirstName = pending.FirstName,
                        LastName = pending.LastName,
                        ShopName = pending.ShopName,
                        PhoneNumber = pending.PhoneNumber,
                        Slug = await GenerateSlugAsync($"{email}", $"{pending.ShopName}", $"{pending.LastName} {pending.FirstName}"),
                        IsDisable = false,
                        TwoFactorEnabled = false
                    };

                    _unitOfWork.PendingAccountRepository.RemoveEntity(pending);
                }
                else
                {
                    response.Message = "Invalid registration!";
                    return response;
                }

                account.Password = HashPassword(account, request.Password);

                await _unitOfWork.AccountRepository.InsertAsync(account);
                await _unitOfWork.CommitAsync();

                await _cartService.CreateCart(account.Id);

                response.Success = true;
                response.Data = account;
                response.Message = "Account registered successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An errror has occured, please try again later!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> LoginAsync(LoginRequest request)
        {
            var response = new BaseResponse();
            try
            {
                // Admin account
                var adminEMail = _configuration["Admin:Email"];
                var adminPassword = _configuration["Admin:Password"];

                if (string.Equals(request.Email, adminEMail, StringComparison.OrdinalIgnoreCase))
                {
                    var adminAccount = await _unitOfWork.AccountRepository.Queryable()
                                            .Where(a => a.Email == adminEMail)
                                            .FirstOrDefaultAsync();

                    if (adminAccount != null)
                    {
                        if (!VerifyPassword(adminAccount, request.Password))
                        {
                            response.Message = "Invalid email or password!";
                            return response;
                        }

                        if (adminAccount.IsDisable == true)
                        {
                            response.Message = "Access Denied!";
                            return response;
                        }
                    }
                    else
                    {
                        if (request.Password != adminPassword)
                        {
                            response.Message = "Invalid password!";
                            return response;
                        }

                        adminAccount = new Account
                        {
                            Email = adminEMail!,
                            FirstName = adminEMail,
                            LastName = "",
                            RoleId = 1,
                            Slug = ""
                        };
                    }

                    var token = await GenerateJwtToken(adminAccount);

                    var adLoginResponse = new LoginResponse
                    {
                        Token = token,
                        RoleId = 1,
                        Role = adminAccount.Role.RoleName,
                        Slug = adminAccount.Slug,
                        FullName = adminAccount.FirstName!,
                        Avatar = adminAccount.Avatar,
                        RequiredTwoFactor = adminAccount.TwoFactorEnabled
                    };

                    response.Success = true;
                    response.Data = adLoginResponse;
                    response.Message = "Admin login successfully.";
                }

                // Other accounts
                var account = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.Email == request.Email)
                                    .Include(a => a.Role)
                                    .FirstOrDefaultAsync();

                if (account == null || !VerifyPassword(account, request.Password))
                {
                    response.Message = "Invalid email or password!";
                    return response;
                }

                if (account.IsDisable == true)
                {
                    response.Message = "Account has been banned";
                    return response;
                }

                if (account.TwoFactorEnabled == true)
                {
                    var otp = GenerateOTP();
                    account.TwoFactorToken = otp;
                    account.TwoFactorTokenExpiry = TimeHelper.VietnamTimeZone().AddMinutes(5);

                    await _unitOfWork.CommitAsync();

                    await SendLoginOTPEmail(account.Email, otp);

                    response.Success = true;
                    response.Data = new { RequiredTwoFactor = true };
                    response.Message = "OTP has been sent to your email.";

                    return response;
                }

                var userToken = await GenerateJwtToken(account);

                var loginResponse = new LoginResponse
                {
                    Token = userToken,
                    RoleId = account.RoleId,
                    Role = account.Role.RoleName,
                    Slug = account.Slug,
                    FullName = account.LastName + " " + account.FirstName,
                    Avatar = account.Avatar,
                    RequiredTwoFactor = false
                };

                response.Success = true;
                response.Data = loginResponse;
                response.Message = "User login successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error attemping to login!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> VerifyLoginOTP(VerifyLoginOTPRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.Email == request.Email && a.TwoFactorEnabled == true)
                                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    response.Message = "Invalid account!";
                    return response;
                }

                if (account.TwoFactorTokenExpiry < TimeHelper.VietnamTimeZone() ||
                    account.TwoFactorToken != request.OTP)
                {
                    response.Message = "Invalid or Expired OTP";
                    return response;
                }

                account.TwoFactorToken = null;
                account.TwoFactorTokenExpiry = null;

                await _unitOfWork.CommitAsync();

                var token = await GenerateJwtToken(account);

                var loginResponse = new LoginResponse
                {
                    Token = token,
                    RoleId = account.RoleId,
                    Role = account.Role.RoleName,
                    Slug = account.Slug,
                    FullName = account.LastName + " " + account.FirstName,
                    Avatar = account.Avatar,
                    RequiredTwoFactor = false
                };

                response.Success = true;
                response.Data = loginResponse;
                response.Message = "OTP verified successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error verfying OTP!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        #region
        private string HashPassword(Account account, string password)
        {
            return _passwordHasher.HashPassword(account, password);
        }

        private bool VerifyPassword(Account account, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(account, account.Password, providedPassword);
            return result == PasswordVerificationResult.Success;
        }

        private async Task<string> GenerateJwtToken(Account account)
        {
            Console.WriteLine($"Generating token for account: {account.Email}");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            string roleName;
            // Id == 0 means admin login from appsettings
            if (account.Id == 0)
            {
                roleName = "Admin";
            }
            else
            {
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(account.RoleId);
                if (role == null)
                {
                    throw new Exception("Role not found");
                }
                roleName = role.RoleName;
            }

            Console.WriteLine($"Role determined: {roleName}");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                        new Claim(ClaimTypes.Email, account.Email),
                        new Claim("id", account.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, roleName),
                        new Claim("name", $"{account.FirstName} {account.LastName}"),
                    }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GenerateSlugAsync(string email, string? shopName, string? fullName)
        {
            string baseText;

            if (!string.IsNullOrWhiteSpace(shopName))
            {
                baseText = shopName;
            }
            else if (!string.IsNullOrWhiteSpace(fullName))
            {
                baseText = fullName;
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                baseText = email.Contains("@")
                    ? email.Split("@")[0]
                    : email;
            }
            else
            {
                baseText = "user";
            }

            // Add "-" before uppercase letter, except first one
            baseText = Regex.Replace(baseText, @"([a-z0-9])([A-Z])", "$1-$2");

            // Distinguish uppercase and lowercase
            baseText = Regex.Replace(baseText, @"([A-Z]+)([A-Z][a-z])", "$1-$2");

            // Add "-" between letter and number
            baseText = Regex.Replace(baseText, @"([a-zA-Z])([0-9])", "$1-$2");
            baseText = Regex.Replace(baseText, @"([0-9])([a-zA-Z])", "$1-$2");

            string slug = Regex.Replace(baseText.ToLower().Trim(), @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-"); // space -> -
            slug = Regex.Replace(slug, @"-+", "-"); // N(-) -> 1(-)
            slug = slug.Trim('-');

            if (string.IsNullOrWhiteSpace(slug))
                slug = "user";

            string uniqueSlug = slug;
            int count = 1;

            while (await _unitOfWork.AccountRepository.Queryable()
                            .Where(a => a.Slug == uniqueSlug)
                            .AnyAsync())
            {
                uniqueSlug = $"{slug}-{count++}";
            }

            return uniqueSlug;
        }

        private string GenerateOTP()
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            uint value = BitConverter.ToUInt32(bytes, 0);

            var otp = (value % 900000 + 100000).ToString();
            return otp;
        }

        private async Task SendLoginOTPEmail(string email, string otp)
        {
            var body = $@"
                <h2>Login Verification</h2>
                <p>Your OTP code is: <strong>{otp}</strong></p>
                <p>This code will be expired in 5 minutes.</p>
                <p>If you didn't attempt to login, please secure your account immediately!</p>";

            await _emailService.SendEmailAsync(email, "Login OTP Verification", body);
        }

        private async Task SendResetPasswordEmail(string email, string otp)
        {
            var body = $@"
                <h2>Reset Your Password</h2>
                <p>This code will be expired in 15 minutes.<p/>
                <p>If you did't make the request, you can ignore this email.<p/>
                <p>For security reasons, please do not share this OTP with anyone.<p/>";

            await _emailService.SendEmailAsync(email, "Reset Password OTP", body);
        }
        #endregion
    }
}

using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using BusinessLogicLayer.Utilities;
using DataAccessObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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

                var existingPending = await _unitOfWork.PendingVendorRepository.Queryable()
                                            .Where(pv => pv.Email == request.Email)
                                            .ToListAsync();

                if (existingPending.Any())
                {
                    _unitOfWork.PendingVendorRepository.RemoveRange(existingPending);
                }

                var pending = new PendingVendor
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    ShopName = request.ShopName,
                    PhoneNumber = request.PhoneNumber,
                    CreateAt = TimeHelper.VietnamTimeZone()
                };

                await _unitOfWork.PendingVendorRepository.InsertAsync(pending);
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
                var role = principal.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

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

                Account account;

                if (role == "customer")
                {
                    account = new Account
                    {
                        Email = email,
                        RoleId = 3,
                        IsDisable = false,
                        TwoFactorEnabled = false
                    };
                }
                else if (role == "vendor")
                {
                    var pending = await _unitOfWork.PendingVendorRepository.Queryable()
                                    .Where(pv => pv.Email == email)
                                    .FirstOrDefaultAsync();

                    if (pending == null)
                    {
                        response.Message = "Registration data not found or expired!";
                        return response;
                    }

                    account = new Account
                    {
                        Email = email,
                        RoleId = 2,
                        FirstName = pending.FirstName,
                        LastName = pending.LastName,
                        ShopName = pending.ShopName,
                        PhoneNumber = pending.PhoneNumber,
                        IsDisable = false,
                        TwoFactorEnabled = false
                    };

                    _unitOfWork.PendingVendorRepository.RemoveEntity(pending);
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
                            RoleId = 1
                        };
                    }

                    var token = await GenerateJwtToken(adminAccount);

                    var adLoginResponse = new LoginResponse
                    {
                        Token = token,
                        RoleId = 1,
                        Role = adminAccount.Role.RoleName                       
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

                var userToken = await GenerateJwtToken(account);

                var loginResponse = new LoginResponse
                {
                    Token = userToken,
                    RoleId = account.RoleId,
                    Role = account.Role.RoleName
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

        public Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> Toggle2FAAsync(Toggle2FARequest request)
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
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new Claim("FullName", $"{account.FirstName} {account.LastName}"),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}

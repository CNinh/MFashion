using BusinessLogicLayer.Interfaces;
using DataAccessObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Account> _passwordHasher;

        public DataSeeder(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<Account>();
        }

        public async Task SeedAdminAsync()
        {
            var email = _configuration["Admin:Email"];
            if (string.IsNullOrEmpty(email) )
            {
                return;
            }

            var existingAccount = await _unitOfWork.AccountRepository.Queryable()
                .Where(a => a.Email == email)
                .FirstOrDefaultAsync();

            if (existingAccount == null)
            {
                var admin = new Account
                {
                    Email = email,
                    FirstName = "Admin",
                    LastName = " ",
                    RoleId = 1,
                    IsDisable = false
                };

                var password = _configuration["Admin:Password"];
                admin.Password = _passwordHasher.HashPassword(admin, password);

                await _unitOfWork.AccountRepository.InsertAsync(admin);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}

using DataAccessObject;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MFashionStoreDBContext _context;

        public UnitOfWork(MFashionStoreDBContext context)
        {
            _context = context;

            AccountRepository = new AccountRepository(_context);
            PendingVendorRepository = new PendingVendorRepository(_context);
            CartRepository = new CartRepository(_context);
            RoleRepository = new RoleRepository(_context);
            ProductRepository = new ProductRepository(_context);
            ProductImageRepository = new ProductImageRepository(_context);
        }

        public IAccountRepository AccountRepository { get; private set; }
        public IPendingVendorRepository PendingVendorRepository { get; private set; }
        public ICartRepository CartRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductImageRepository ProductImageRepository { get; private set; }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task CommitAsync()
            => await _context.SaveChangesAsync();

        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}

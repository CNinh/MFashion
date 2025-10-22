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
            PendingAccountRepository = new PendingAccountRepository(_context);
            CartRepository = new CartRepository(_context);
            CartItemRepository = new CartItemRepository(_context);
            RoleRepository = new RoleRepository(_context);
            ProductRepository = new ProductRepository(_context);
            ProductImageRepository = new ProductImageRepository(_context);
            OrderRepository = new OrderRepository(_context);
            OrderDetailRepository = new OrderDetailRepository(_context);
            ColorRepository = new ColorRepository(_context);
            SizeRepository = new SizeRepository(_context);
            MaterialRepository = new MaterialRepository(_context);
            DeliveryRepository = new DeliveryRepository(_context);
            TagRepository = new TagRepository(_context);
            DesignRepository = new DesignRepository(_context);
            CategoryRepository = new CategoryRepository(_context);
            BlogCategoryRepository = new BlogCategoryRepository(_context);
            BlogRepository = new BlogRepository(_context);
            BlogImageRepository = new BlogImageRepository(_context);
            BlogTagRepository = new BlogTagRepository(_context);
            CommentRepository = new CommentRepository(_context);
        }

        public IAccountRepository AccountRepository { get; private set; }
        public IPendingAccountRepository PendingAccountRepository { get; private set; }
        public ICartRepository CartRepository { get; private set; }
        public ICartItemRepository CartItemRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductImageRepository ProductImageRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IOrderDetailRepository OrderDetailRepository { get; private set; }
        public IColorRepository ColorRepository { get; private set; }
        public ISizeRepository SizeRepository { get; private set; }
        public IMaterialRepository MaterialRepository { get; private set; }
        public IDeliveryRepository DeliveryRepository { get; private set; }
        public ITagRepository TagRepository { get; private set; }
        public IDesignRepository DesignRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public IBlogCategoryRepository BlogCategoryRepository { get; private set; }
        public IBlogRepository BlogRepository { get; private set; }
        public IBlogImageRepository BlogImageRepository { get; private set; }
        public IBlogTagRepository BlogTagRepository { get; private set; }
        public ICommentRepository CommentRepository { get; private set; }

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

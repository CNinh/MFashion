using DataAccessObject.Model;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IPendingAccountRepository PendingAccountRepository { get; }
        ICartRepository CartRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        IRoleRepository RoleRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IColorRepository ColorRepository { get; }
        ISizeRepository SizeRepository { get; }
        IMaterialRepository MaterialRepository { get; }
        IDeliveryRepository DeliveryRepository { get; }
        ITagRepository TagRepository { get; }
        IDesignRepository DesignRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IBlogCategoryRepository BlogCategoryRepository { get; }
        IBlogRepository BlogRepository { get; }
        IBlogImageRepository BlogImageRepository { get; }
        IBlogTagRepository BlogTagRepository { get; }
        ICommentRepository CommentRepository { get; }

        int Save();
        Task CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}

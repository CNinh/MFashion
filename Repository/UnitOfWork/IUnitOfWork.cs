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
        IPendingVendorRepository PendingVendorRepository { get; }
        ICartRepository CartRepository { get; }
        IRoleRepository RoleRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IColorRepository ColorRepository { get; }
        ISizeRepository SizeRepository { get; }
        IMaterialRepository MaterialRepository { get; }
        IDeliveryRepository DeliveryRepository { get; }
        ITagRepository TagRepository { get; }
        int Save();
        Task CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}

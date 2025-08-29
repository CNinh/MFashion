using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelResponse;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<BaseResponse> CancelOrder(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> CreateOrder()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> GetOrderList(int id)
        {
            throw new NotImplementedException();
        }
    }
}

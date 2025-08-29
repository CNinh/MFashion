using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IOrderService
    {
        Task<BaseResponse> GetOrderList(int id);
        Task<BaseResponse> GetOrderById(int id);
        Task<BaseResponse> CreateOrder();
        Task<BaseResponse> CancelOrder(int id);
    }
}

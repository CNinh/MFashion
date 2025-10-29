using BusinessLogicLayer.ModelRequest;
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
        Task<BaseResponse<PageResult<OrderResponse>>> GetOrderList(OrderRequest request);
        Task<BaseResponse<DetailResponse>> GetOrderById(int id);
        Task<BaseResponse> CreateOrder(int id, CreateOrderRequest request);
        Task<BaseResponse> ChangeAddress(int id, AddressRequest request);
        Task<BaseResponse> CancelOrder(int id);
        Task<BaseResponse> UpdateStatus(int id);
    }
}

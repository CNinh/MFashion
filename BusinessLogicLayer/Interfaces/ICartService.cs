using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICartService
    {
        Task<BaseResponse> CreateCart(int id);
        Task<BaseResponse> GetCart(int id);
        Task<BaseResponse> AddToCart(int id, AddToCartRequest request);
        Task<BaseResponse> UpdateQuantity(int id, UpdateQuantityRequest request);
        Task<BaseResponse> RemoveFromCart(int id, RemoveProductRequest request);
        Task<BaseResponse> ClearItem(int id);
    }
}

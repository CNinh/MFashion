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
        Task<BaseResponse> AddToCart(int id, int quantity);
        Task<BaseResponse> UpdateQuantity(int id, int productId, int quantity);
        Task<BaseResponse> RemoveFromCart(int id, int productId);
        Task<BaseResponse> ClearItem(int id, int productId);
    }
}

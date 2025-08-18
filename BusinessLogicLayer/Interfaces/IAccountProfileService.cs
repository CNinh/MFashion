using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IAccountProfileService
    {
        Task<BaseResponse> GetAccountBySlug(string slug);
        Task<BaseResponse> UpdateProfileAsync(int id, UpdateProfileRequest request);
        Task<BaseResponse> UpdateAvatarAsync(int id, Stream fileStream, string fileName);
        Task<BaseResponse> UpdateShopAsync(int id, string shopName);
        Task<BaseResponse> Toggle2FAAsync(int id);
    }
}

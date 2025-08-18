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
        Task<BaseResponse> GetAllAccount();
        Task<BaseResponse> GetAccountById(int id);
        Task<BaseResponse> UpdateProfileAsync(int id, Stream fileStream, string fileName);
        Task<BaseResponse> DeleteAvatarAsync(int id);
    }
}

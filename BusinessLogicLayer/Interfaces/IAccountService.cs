using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse> GetAllAccount();
        Task<BaseResponse> GetAccountById(int id);
        Task<BaseResponse> DisableAccount(int id);
    }
}

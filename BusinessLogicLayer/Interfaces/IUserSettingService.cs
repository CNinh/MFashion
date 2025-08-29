using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserSettingService
    {
        Task<BaseResponse> GetUserSetting(int id);
        Task<BaseResponse> Toggle2FAAsync(int id);
    }
}

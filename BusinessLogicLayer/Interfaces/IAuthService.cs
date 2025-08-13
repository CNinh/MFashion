using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse> RegisterCustomerAsync(RegisterRequest request);
        Task<BaseResponse> RegisterVendorAsync(RegisterVendorRequest request);
        Task<BaseResponse> SetupPasswordAsync(SetupPasswordRequest request);
        Task<BaseResponse> LoginAsync(LoginRequest request);
        Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<BaseResponse> Toggle2FAAsync(Toggle2FARequest request);
    }
}

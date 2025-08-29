using BusinessLogicLayer.Interfaces;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayer.ModelResponse;

namespace BusinessLogicLayer.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserSettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> GetUserSetting(int id)
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.Queryable()
                                .Where(a => a.Id == id && a.RoleId != 1)
                                .FirstOrDefaultAsync();
            
                if (account == null)
                {
                    response.Message = "Invalid account!";
                    return response;
                }

                var status = account.TwoFactorEnabled
                             ? "Enabled"
                             : "Disabled";

                var settingResponse = new UserSettingResponse
                {
                    Id = id,
                    Slug = account.Slug,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    TwoFactorStatus = status,
                };

                response.Success = true;
                response.Data = settingResponse;
                response.Message = "User setting retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving user setting!";
                response.Errors.Add(ex.Message);
            }
            
            return response;
        }

        public async Task<BaseResponse> Toggle2FAAsync(int id)
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.Id == id && a.RoleId != 1)
                                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    response.Message = "Invalid account!";
                    return response;
                }

                if (account.TwoFactorEnabled == false)
                {
                    account.TwoFactorEnabled = true;
                }
                else
                {
                    account.TwoFactorEnabled = false;
                }

                await _unitOfWork.CommitAsync();

                var status = account.TwoFactorEnabled
                             ? "enabled"
                             : "disable";

                response.Success = true;
                response.Data = new { Status = account.TwoFactorEnabled ? "Enabled" : "Disabled" };
                response.Message = $"2FA {status} successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error toggle 2FA!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}

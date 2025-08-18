using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelResponse;
using Microsoft.EntityFrameworkCore;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> GetAllAccount()
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.GetAllAsync();

                var accountList = account.Select(a => new AccountListResponse
                {
                    Id = a.Id,
                    Avatar = a.Avatar,
                    Email = a.Email,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Slug = a.Slug,
                    IsDisable = a.IsDisable
                }).ToList();

                response.Success = true;
                response.Data = accountList;
                response.Message = "Account retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving account list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> GetAccountById(int id)
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.Id == id)
                                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    response.Message = "Account not found!";
                    return response;
                }

                var accountResponse = new AccountResponse
                {
                    Id = account.Id,
                    Slug = account.Slug,
                    Avatar = account.Avatar,
                    Email = account.Email,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    DateOfBirth = account.DateOfBirth,
                    Gender = account.Gender,
                    PhoneNumber = account.PhoneNumber,
                    ShopName = account.ShopName,
                    IsDisalbe = account.IsDisable
                };

                response.Success = true;
                response.Data = accountResponse;
                response.Message = "Account detail retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving account list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> DisableAccount(int id)
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.Id == id)
                                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    response.Message = "Account not found!";
                    return response;
                }

                if (account.IsDisable == false)
                {
                    account.IsDisable = true;
                }
                else
                {
                    account.IsDisable = false;
                }

                var status = account.IsDisable
                             ? "disabled"
                             : "enabled";

                _unitOfWork.AccountRepository.Update(account);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Data = status;
                response.Message = $"Accout {status} successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error disable account!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}

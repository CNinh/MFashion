using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
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
    public class AccountProfileService : IAccountProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public AccountProfileService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse> GetAccountBySlug(string slug)
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.Slug == slug)
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

        public async Task<BaseResponse> UpdateProfileAsync(int id, UpdateProfileRequest request)
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

                account.FirstName = request.FirstName;
                account.LastName = request.LastName;
                account.PhoneNumber = request.PhoneNumber;
                account.Slug = request.Slug;
                account.DateOfBirth = request.DateOfBirth;

                _unitOfWork.AccountRepository.Update(account);
                await _unitOfWork.CommitAsync();

                var accountResponse = new UpdateProfileResponse
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Slug = request.Slug,
                };

                response.Success = true;
                response.Data = accountResponse;
                response.Message = "Account profile updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating account profile!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> UpdateAvatarAsync(int id, Stream fileStream, string fileName)
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

                var image = await _cloudinaryService.UploadAvatarAsync(fileStream, fileName);
                account.Avatar = image;

                _unitOfWork.AccountRepository.Update(account);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Data = image;
                response.Message = "Avatar updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating avatar!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> UpdateShopAsync(int id, string shopName)
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

                if (account.RoleId != 2)
                {
                    response.Message = "Invalid role!";
                    return response;
                }

                var existingName = await _unitOfWork.AccountRepository.Queryable()
                                            .Where(es => es.ShopName == shopName)
                                            .FirstOrDefaultAsync();

                if (existingName != null)
                {
                    response.Message = "Name already be taken!";
                    return response;
                }

                account.ShopName = shopName;

                _unitOfWork.AccountRepository.Update(account);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Data = shopName;
                response.Message = "Shop name updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating Shop name!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}

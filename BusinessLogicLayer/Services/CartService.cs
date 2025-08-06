using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelResponse;
using DataAccessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> CreateCart(int id)
        {
            var response = new BaseResponse();
            try
            {
                var existingCart = await _unitOfWork.CartRepository.Queryable()
                                        .Where(c => c.AccountId == id)
                                        .FirstOrDefaultAsync();

                if (existingCart != null)
                {
                    response.Message = "This account already has cart!";
                    return response;
                }

                var cart = new Cart
                {
                    TotalItem = 0,
                    TotalPrice = 0,
                    AccountId = id
                };

                await _unitOfWork.CartRepository.InsertAsync(cart);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Data = cart;
                response.Message = "Cart created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error creating cart!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}

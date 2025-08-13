using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using DataAccessObject.Model;
using LinqKit;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly CloudinaryService _cloudinaryService;

        public ProductService(UnitOfWork unitOfWork, CloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse<PageResult<ProductListResponse>>> GetProductList(ProductListRequest request)
        {
            var response = new BaseResponse<PageResult<ProductListResponse>>();
            try
            {
                // Filter
                Expression<Func<Product, bool>> filter = product =>
                    (string.IsNullOrEmpty(request.ProductName) || product.ProductName.Contains(request.ProductName)) &&
                    (!request.MinPrice.HasValue || product.Price >= request.MinPrice.Value) &&
                    (!request.MaxPrice.HasValue || product.Price <= request.MaxPrice.Value);

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(request.UserId);

                // Check user
                if ((account != null && account.RoleId == 1) || (account != null && account.RoleId == 2))
                {
                    // Display all product
                }
                else
                {
                    filter = filter.And(p => p.Quantity > 0); // Display in stock product
                }

                // Sort
                Expression<Func<Product, object>> orderByExpression = request.SortBy?.ToLower() switch
                {
                    "productname" => p => p.ProductName,
                    "price" => p => p.Price,
                    "createat" => p => p.CreateAt,
                    _ => p => p.Id
                };


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching product list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> GetProductById(int id)
        {
            var response = new BaseResponse();
            try
            {

            }
            catch(Exception ex )
            {
                response.Success = false;
                response.Message = "Error fetching product detail!";
                response.Errors.Add(ex.Message);
            }

            return response; 
        }

        public async Task<BaseResponse> CreateProduct(CreateProductRequest request)
        {
            var response = new BaseResponse();
            try
            {

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error creating product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> UpdateProduct(int id, UpdateProductRequest request)
        {
            var response = new BaseResponse();
            try
            {

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> DeleteProduct(int id)
        {
            var response = new BaseResponse();
            try
            {

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}

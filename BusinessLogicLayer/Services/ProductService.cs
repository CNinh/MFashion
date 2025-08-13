using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
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

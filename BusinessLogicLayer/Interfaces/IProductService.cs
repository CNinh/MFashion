using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProductService
    {
        Task<BaseResponse<PageResult<ProductListResponse>>> GetProductList(int? id, ProductListRequest request);
        Task<BaseResponse<PageResult<ProductListResponse>>> GetProductByVendor(int? id, GetProductByVendorRequest request);
        Task<BaseResponse> GetProductById (int id);
        Task<BaseResponse> GetCreateOption();
        Task<BaseResponse> CreateProduct (int accountId, CreateProductRequest request);
        Task<BaseResponse> UpdateProduct (int id, UpdateProductRequest request);
        Task<BaseResponse> DeleteProduct (int id);
    }
}

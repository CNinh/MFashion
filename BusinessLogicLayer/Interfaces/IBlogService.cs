using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IBlogService
    {
        Task<BaseResponse<PageResult<BlogResponse>>> GetAllBlog(BlogRequest request);
        Task<BaseResponse<BlogDetailResponse>> GetBlogById(int id);
        Task<BaseResponse> CreateBlog(int accountId, CreateBlogRequest request);
        Task<BaseResponse> UpdateBlog(int id, UpdateBlogRequest);
        Task<BaseResponse> DeleteBlog(int id);
        Task<BaseResponse> Comment(int id, int accountId, CommmentRequest request);
    }
}

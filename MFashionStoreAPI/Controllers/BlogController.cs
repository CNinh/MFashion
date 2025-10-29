using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MFashionStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpPost("getBlogList")]
        public async Task<IActionResult> GetAllBlog([FromBody] BlogRequest request)
        {
            var result = await _blogService.GetAllBlog(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getBlogById/{id}")]
        public async Task<IActionResult> GetBlogDetail(int id)
        {
            var result = await _blogService.GetBlogById(id);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("createBlog")]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest request)
        {
            int accountId = GetUserId();

            var result = await _blogService.CreateBlog(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("updateBlog")]
        public async Task<IActionResult> UpdateBlog([FromQuery] int id, [FromBody] UpdateBlogRequest request)
        {
            var result = await _blogService.UpdateBlog(id, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("deleteBlog")]
        public async Task<IActionResult> DeleteBlog([FromQuery] int id)
        {
            var result = await _blogService.DeleteBlog(id);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("commentBlog")]
        public async Task<IActionResult> CommentBlog([FromQuery] int id, [FromBody] CommmentRequest request)
        {
            int accountId = GetUserId();

            var result = await _blogService.Comment(id, accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MFashionStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        private string GetAccountId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost("getProductList")]
        public async Task<IActionResult> GetProductList([FromQuery] ProductListRequest request)
        {
            int? accountId = string.IsNullOrEmpty(GetAccountId()) ? null : int.Parse(GetAccountId());

            var result = await _productService.GetProductList(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("getProductByVendor")]
        public async Task<IActionResult> GetProductByVendor([FromQuery] GetProductByVendorRequest request)
        {
            int? accountId = string.IsNullOrEmpty(GetAccountId()) ? null : int.Parse(GetAccountId());

            var result = await _productService.GetProductByVendor(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductById(id);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Vendor")]
        [HttpGet("getCreateOption")]
        public async Task<IActionResult> GetCreateOption()
        {
            var result = await _productService.GetCreateOption();

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Vendor")]
        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _productService.CreateProduct(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Vendor")]
        [HttpPut("updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductRequest request)
        {
            var result = await _productService.UpdateProduct(id, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Vendor")]
        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

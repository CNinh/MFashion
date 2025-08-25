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

        [HttpGet("getProductList")]
        public async Task<IActionResult> GetProductList([FromQuery] ProductListRequest request)
        {
            var result = await _productService.GetProductList(request);

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

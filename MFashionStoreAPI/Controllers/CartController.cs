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
    [Authorize(Policy = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpGet("getCart")]
        public async Task<IActionResult> GetCart()
        {
            int accountId = GetUserId();

            var result = await _cartService.GetCart(accountId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("addProduct")]
        public async Task<IActionResult> AddToCart([FromForm] AddToCartRequest request)
        {
            int accountId = GetUserId();

            var result = await _cartService.AddToCart(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("updateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            int accountId = GetUserId();

            var result = await _cartService.UpdateQuantity(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("removeProduct")]
        public async Task<IActionResult> RemoveProduct([FromBody] RemoveProductRequest request)
        {
            int accountId = GetUserId();

            var result = await _cartService.RemoveFromCart(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("clearCart")]
        public async Task<IActionResult> ClearCart()
        {
            int accountId = GetUserId();

            var result = await _cartService.ClearItem(accountId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

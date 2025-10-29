using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.Security.Claims;

namespace MFashionStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [Authorize(Policy = "AllRoles")]
        [HttpPost("getOrderList")]
        public async Task<IActionResult> GetOrderList([FromBody] OrderRequest request)
        {
            var result = await _orderService.GetOrderList(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "AllRoles")]
        [HttpGet("getOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderById(id);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Customer")]
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var accountId = GetUserId();

            var result = await _orderService.CreateOrder(accountId, request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Customer")]
        [HttpPut("changeAddress/{id}")]
        public async Task<IActionResult> ChangeAddress(int id, [FromBody] AddressRequest request)
        {
            var result = await _orderService.ChangeAddress(id, request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Customer")]
        [HttpPut("cancelOrder/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrder(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "Vendor")]
        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result = await _orderService.UpdateStatus(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

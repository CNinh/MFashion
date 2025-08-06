using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MFashionStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterCustomerAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("confirm-customer")]
        public async Task<IActionResult> ConfirmCustomer([FromBody] ConfirmRegistrationRequest request)
        {
            var result = await _authService.ConfirmCustomerAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("register-vendor")]
        public async Task<IActionResult> RegisterVendor([FromBody] RegisterVendorRequest request)
        {
            var result = await _authService.RegisterVendorAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("confirm-vendor")]
        public async Task<IActionResult> ConfirmVendor([FromBody] ConfirmRegistrationRequest request)
        {
            var result = await _authService.ConfirmVendorAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

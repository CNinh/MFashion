using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MFashionStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class AccountManagementController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountManagementController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("getAllAccount")]
        public async Task<IActionResult> GetAllAccount()
        {
            var result = await _accountService.GetAllAccount();

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        [HttpGet("getAccountById/{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var result = await _accountService.GetAccountById(id);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("toggle-status")]
        public async Task<IActionResult> ToggleStatus([Required] int id)
        {
            var result = await _accountService.DisableAccount(id);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MFashionStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User")]
    public class UserSettingController : ControllerBase
    {
        private readonly IUserSettingService _settingService;

        public UserSettingController(IUserSettingService settingService)
        {
            _settingService = settingService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpGet("getSetting")]
        public async Task<IActionResult> GetUserSetting()
        {
            int accountId = GetUserId();

            var result = await _settingService.GetUserSetting(accountId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("toggle-2FA")]
        public async Task<IActionResult> Toggle2FA()
        {
            int accountId = GetUserId();

            var result = await _settingService.Toggle2FAAsync(accountId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

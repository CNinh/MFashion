using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MFashionStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllRoles")]
    public class AccountProfileController : ControllerBase
    {
        private readonly IAccountProfileService _accountProfileService;

        public AccountProfileController(IAccountProfileService accountProfileService)
        {
            _accountProfileService = accountProfileService;
        }

        [HttpGet("getProfile")]
        public async Task<IActionResult> GetAccountBySlug([FromQuery, Required] string slug)
        {
            var result = await _accountProfileService.GetAccountBySlug(slug);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _accountProfileService.UpdateProfileAsync(accountId, request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("updateAvatar")]
        public async Task<IActionResult> UpdateAvatar([Required] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            using var stream = file.OpenReadStream();
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var result = await _accountProfileService.UpdateAvatarAsync(accountId, stream, fileName);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("updateShopName")]
        public async Task<IActionResult> UpdateShop([FromQuery, Required] string name)
        {
            int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _accountProfileService.UpdateShopAsync(accountId, name);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

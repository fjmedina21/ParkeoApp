using ParkeoApp.Api.Models.ApiResponses;
using ParkeoApp.Api.Models.DTO;
using ParkeoApp.Api.Services.AuthService;
using ParkeoApp.Api.Services.AuthService.DTO;
using ParkeoApp.Api.Services.TenantService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ParkeoApp.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType<ApiResponse<GetUser>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<GetUser>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] CredentialsDto model)
        {
            HttpContext ht = HttpContext;
            var response = await authService.LoginAsync(model, ht);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType<ApiResponse<GetUser>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ApiResponse<GetUser>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Signup([FromBody] AddUser model)
        {
            var ht = HttpContext;
            ApiResponse<GetUser> response = await authService.SignupAsync(model, ht);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("change-password")]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            string token = Request.Headers["Authorization"]!;
            ApiResponse response = await authService.ChangePasswordAsync(model, token);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            string token = Request.Headers["Authorization"]!;
            ApiResponse response = await authService.ForgotPasswordAsync(model);
            return StatusCode(response.StatusCode, response);
        }
    }
}

using ParkeoApp.Api.Models.ApiResponses;
using ParkeoApp.Api.Models.DTO;
using ParkeoApp.Api.Services.AuthService.DTO;

namespace ParkeoApp.Api.Services.AuthService
{
	public interface IAuthService
	{
		Task<ApiResponse<GetUser>> LoginAsync(CredentialsDto login, HttpContext httpContext);
		Task<ApiResponse<GetUser>> SignupAsync(AddUser signup,HttpContext httpContext);
		Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto model, string token);
		Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto model);
	}
}

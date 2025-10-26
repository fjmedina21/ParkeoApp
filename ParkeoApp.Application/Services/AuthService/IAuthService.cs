using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using Microsoft.AspNetCore.Http;
using ParkeoApp.Application.Services.AuthService.DTO;

namespace ParkeoApp.Application.Services.AuthService
{
	public interface IAuthService
	{
		Task<ApiResponse<GetUser>> LoginAsync(CredentialsDto login, HttpContext httpContext);
		Task<ApiResponse<GetUser>> SignupAsync(AddUser signup,HttpContext httpContext);
		Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto model, string token);
		Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto model);
	}
}

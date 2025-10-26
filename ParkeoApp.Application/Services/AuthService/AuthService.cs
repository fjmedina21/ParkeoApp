using AutoMapper;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Services.AuthService.DTO;

namespace ParkeoApp.Application.Services.AuthService
{
	public class AuthService(ParkeoAppContext dbContext, IMapper mapper, IConfiguration configuration, ILogger<AuthService> logger) : IAuthService
	{
		public async Task<ApiResponse<GetUser>> LoginAsync(CredentialsDto credentials, HttpContext httpContext)
		{
			user? user = await dbContext.users.AsNoTracking()
				.Include(e => e.user_roles)
				.Where(e => e.is_active)
				.FirstOrDefaultAsync(e => e.email.Equals(credentials.Email));

			if (user is null || !Utils.CompareText(credentials.Password, user.password_hash))
				return new ApiResponse<GetUser>(statusCode: StatusCodes.Status400BadRequest, message: "Invalid credentials. Please try again.");

			var dto = mapper.Map<GetUser>(user);
			await HandleTokenGenerationAndStorage(httpContext, user);
			return new ApiResponse<GetUser>(data: [dto]);
		}

		public async Task<ApiResponse<GetUser>> SignupAsync(AddUser signup, HttpContext httpContext)
		{
			user newUser = mapper.Map<user>(signup);

			if (await Validations.EmailExist(signup.email, dbContext)) return new ApiResponse<GetUser>(statusCode: StatusCodes.Status400BadRequest, message: "Email already exists.");

			newUser.password_hash = Utils.HashText(signup.password);
			var entry = await dbContext.users.AddAsync(newUser);
			entry.Entity.is_active = true;
			await dbContext.SaveChangesAsync();

			await HandleTokenGenerationAndStorage(httpContext, entry.Entity);
			GetUser? createdUser = mapper.Map<GetUser>(entry.Entity);

			return new ApiResponse<GetUser>(statusCode: StatusCodes.Status201Created,  data: [createdUser]);
		}

		public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto model, string token)
		{
			var payload = Utils.DecodeJwt(token);

			user? user = await dbContext.users.Where(e => e.is_active)
				.FirstOrDefaultAsync(e => e.user_id.Equals(payload.User));

			bool currentPasswordMatch = Utils.CompareText(model.OldPassword, user!.password_hash);
			bool newPasswordMatch = Utils.CompareText(model.NewPassword, user.password_hash);

			if (!currentPasswordMatch)
				return new ApiResponse(
					statusCode: StatusCodes.Status400BadRequest, message: "Current password does not match.");
			if (newPasswordMatch)
				return new ApiResponse(
					statusCode: StatusCodes.Status400BadRequest, message: "Try a password different than the current one.");

			user.password_hash = Utils.HashText(model.NewPassword);
			await dbContext.SaveChangesAsync();

			return new ApiResponse(message: "Password changed successfully.");
		}

		public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto model)
		{
            logger.LogInformation("Forgot password called");

			return new ApiResponse(message: $"check your email {model.Email} for further instructions");
		}

		private async Task<string> HandleTokenGenerationAndStorage(HttpContext httpContext, user user)
		{
			string sessionJwtAsync = Utils.GenerateSessionJwtAsync(user, configuration);
			string refreshJwtAsync = Utils.GenerateRefreshJwtAsync(user, configuration);
			httpContext.Response.Headers["jwt"] = sessionJwtAsync;
			httpContext.Response.Headers["refresh-jwt"] = refreshJwtAsync;
			await dbContext.users_tokens.AddAsync(new users_token()
			{
				tenant_id = user.tenant_id,
				user_id = user.user_id,
				access_token = sessionJwtAsync,
				refresh_token = refreshJwtAsync,
				access_expires_at = Utils.DecodeJwt(sessionJwtAsync).ExpiresIn,
				refresh_expires_at =Utils.DecodeRefreshJwt(sessionJwtAsync).ExpiresIn,
			});
			await dbContext.SaveChangesAsync();
			return sessionJwtAsync;
		}
	}
}

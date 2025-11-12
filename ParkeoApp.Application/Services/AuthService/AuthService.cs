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
			User? user = await dbContext.Users.AsNoTracking()
				.Include(e => e.UserRoles)
				.Where(e => !e.DeletedAt.HasValue)
				.FirstOrDefaultAsync(e => e.Email.Equals(credentials.Email));

			if (user is null || !Utils.CompareText(credentials.Password, user.PasswordHash))
				return new ApiResponse<GetUser>(statusCode: StatusCodes.Status400BadRequest, message: "Invalid credentials. Please try again.");

			GetUser? dto = mapper.Map<GetUser>(user);
			await HandleTokenGenerationAndStorage(httpContext, user);
			return new ApiResponse<GetUser>(data: [dto]);
		}

		public async Task<ApiResponse<GetUser>> SignupAsync(AddUser signup, HttpContext httpContext)
		{
			User newUser = mapper.Map<User>(signup);
			if (await Validations.EmailExist(signup.Email, dbContext)) return new ApiResponse<GetUser>(statusCode: StatusCodes.Status400BadRequest, message: "Email already exists.");
			Tenant? tenant = await dbContext.Tenants.FirstOrDefaultAsync(e => e.TenantId.Equals(signup.TenantId));

			string domain = signup.Email.Split("@")[^1];
			if (tenant is not null && !tenant.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase)) return new ApiResponse<GetUser>(statusCode:StatusCodes.Status400BadRequest,message:"The specified domain does not match the tenant configuration.");

			newUser.PasswordHash = Utils.HashText(signup.Password);
			var entry = await dbContext.Users.AddAsync(newUser);
			await dbContext.SaveChangesAsync();

			await HandleTokenGenerationAndStorage(httpContext, entry.Entity);
			GetUser? createdUser = mapper.Map<GetUser>(entry.Entity);

			return new ApiResponse<GetUser>(statusCode: StatusCodes.Status201Created,  data: [createdUser]);
		}

		public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto model, string token)
		{
			TokenPayload payload = Utils.DecodeJwt(token);
			User? user = await dbContext.Users.Where(e => !e.DeletedAt.HasValue)
				.FirstOrDefaultAsync(e => e.UserId.Equals(payload.User));

			bool currentPasswordMatch = Utils.CompareText(model.OldPassword, user!.PasswordHash);
			bool newPasswordMatch = Utils.CompareText(model.NewPassword, user.PasswordHash);

			if (!currentPasswordMatch)
				return new ApiResponse(
					statusCode: StatusCodes.Status400BadRequest, message: "Current password does not match.");
			if (newPasswordMatch)
				return new ApiResponse(
					statusCode: StatusCodes.Status400BadRequest, message: "Try a password different than the current one.");

			user.PasswordHash = Utils.HashText(model.NewPassword);
			await dbContext.SaveChangesAsync();

			return new ApiResponse(message: "Password changed successfully.");
		}

		public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto model)
		{
			User? user = await dbContext.Users
				.Where(e => !e.DeletedAt.HasValue)
				.FirstOrDefaultAsync(e => e.Email.Equals(model.Email));

			if (user is null) return new ApiResponse(statusCode: StatusCodes.Status400BadRequest, message: "Email not found.");

			// set a temporary password
			Random rnd = new();
			var randomNumberInRange = rnd.Next(100000, 999999).ToString();

			user.PasswordHash = Utils.HashText(randomNumberInRange);
			await dbContext.SaveChangesAsync();

			// send email
			string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "send-code-by-email-template.html");
			string htmlFile = await File.ReadAllTextAsync(templatePath);
			const string subject = "Forgot Password - New Password";
			string htmlBody = htmlFile
				.Replace("{{subject}}", subject)
				.Replace("{{code}}", randomNumberInRange);

			var mail = new EmailReq(To: [user.Email], Subject: subject, Body: htmlBody);
			Utils.SendEmail(mail, configuration);

			return new ApiResponse(message: $"check your email {model.Email} for further instructions");
		}

		private async Task<string> HandleTokenGenerationAndStorage(HttpContext httpContext, User user)
		{
			string sessionJwtAsync = Utils.GenerateSessionJwtAsync(user, configuration);
			string refreshJwtAsync = Utils.GenerateRefreshJwtAsync(user, configuration);

			httpContext.Response.Headers["jwt"] = sessionJwtAsync;
			httpContext.Response.Headers["refresh-jwt"] = refreshJwtAsync;

			await dbContext.UsersTokens.AddAsync(new UsersToken()
			{
				TenantId = user.TenantId,
				UserId = user.UserId,
				AccessToken = sessionJwtAsync,
				RefreshToken = refreshJwtAsync,
				AccessExpiresAt = Utils.DecodeJwt(sessionJwtAsync).ExpiresIn,
				RefreshExpiresAt = Utils.DecodeRefreshJwt(sessionJwtAsync).ExpiresIn,
			});

			await dbContext.SaveChangesAsync();
			return sessionJwtAsync;
		}
	}
}

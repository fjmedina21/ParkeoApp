using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using BC = BCrypt.Net.BCrypt;

namespace ParkeoApp.Application.Helpers
{
	public static class Utils
	{
		#region JWT
		public static string GenerateSessionJwtAsync(user entity, IConfiguration configuration)
		{
			var key = Encoding.UTF8.GetBytes(configuration["JWTSettings:key"]!);
			var validHours = int.Parse(configuration["JWTSettings:expiresIn-hours"]!);

			List<Claim> authClaims =
			[
				new Claim("userId", entity.user_id.ToString()),
				new Claim("tenantId", entity.tenant_id.ToString()),
			];

			var secretKey = new SymmetricSecurityKey(key);
			var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
			var token = new JwtSecurityToken(claims: authClaims, expires: DateTime.Now.AddHours(validHours), signingCredentials: credentials);
			var handledToken = new JwtSecurityTokenHandler().WriteToken(token);

			return handledToken;
		}

		public static string GenerateRefreshJwtAsync(user entity, IConfiguration configuration)
		{
			var key = Encoding.UTF8.GetBytes(configuration["JWTSettings:key"]!);
			var validHours = int.Parse(configuration["JWTSettings:expiresIn-refreshToken-hours"]!);

			List<Claim> authClaims =
			[
				new Claim("userId", entity.user_id.ToString())
			];

			var secretKey = new SymmetricSecurityKey(key);
			var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
			var token = new JwtSecurityToken(claims: authClaims, expires: DateTime.Now.AddHours(validHours), signingCredentials: credentials);
			var handledToken = new JwtSecurityTokenHandler().WriteToken(token);

			return handledToken;
		}

		public static TokenPayload DecodeJwt(string token)
		{
			string[] bearerToken = token.Split(' ');
			JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(bearerToken[^1]);

			var to = new TokenPayload(
				User: Guid.Parse(jwt.Claims.First(c => c.Type == "userId").Value),
				Tenant: Guid.Parse(jwt.Claims.First(c => c.Type == "tenantId").Value),
				ExpiresIn: DateTimeOffset.FromUnixTimeSeconds(long.Parse(jwt.Claims.First(c => c.Type == "exp").Value)).UtcDateTime
			);

			return to;
		}
		public static RefreshTokenPayload DecodeRefreshJwt(string token)
		{
			string[] bearerToken = token.Split(' ');
			JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(bearerToken[^1]);

			var to = new RefreshTokenPayload(
				User: Guid.Parse(jwt.Claims.First(c => c.Type == "userId").Value),
				ExpiresIn: DateTimeOffset.FromUnixTimeSeconds(long.Parse(jwt.Claims.First(c => c.Type == "exp").Value)).UtcDateTime
			);

			return to;
		}

		public static string GenerateResetPasswordJwtAsync(string email, IConfiguration configuration)
		{
			var key = Encoding.UTF8.GetBytes(configuration["JWTSetting:resetPasswordKey"]!);
			var validMinutes = int.Parse(configuration["JWTSetting:resetPasswordTokenExpiresIn-minutes"]!);

			List<Claim> resetPasswordClaims =
			[
				new("email", email)
			];

			var secretKey = new SymmetricSecurityKey(key);
			var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.Aes256CbcHmacSha512);
			var token = new JwtSecurityToken(claims: resetPasswordClaims, expires: DateTime.Now.AddMinutes(validMinutes), signingCredentials: credentials);
			var handledToken = new JwtSecurityTokenHandler().WriteToken(token);

			return handledToken;
		}

		public static string DecodeResetPasswordJwt(string token)
		{
			JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
			string email = jwt.Claims.First(c => c.Type == "email").Value;
			return email;
		}

		public static bool ValidateExpJwt(string token)
		{
			JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

			int exp = int.Parse(jwt.Claims.First(c => c.Type == "exp").Value);
			long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			return currentTime < exp;
		}

		#endregion

		#region BCrypt
		public static string HashText(string txt, int salt = 11)
		{
			return BC.EnhancedHashPassword(inputKey: txt, workFactor: salt);
		}

		public static bool CompareText(string txt, string hashTxt)
		{
			return BC.EnhancedVerify(text: txt, hash: hashTxt);
		}

		#endregion

		// public async static Task SendVerificationCode( User user ,IConfiguration configuration)
		// {
		// 	Random rnd = new();
		// 	var randomNumberInRange = rnd.Next(100000, 999999).ToString();
		//
		// 	user.VerificationCode = HashText(randomNumberInRange);
		// 	user.VerificationCodeValidUntil = DateTime.UtcNow.AddMinutes(10);
		// 	user.VerificationCodeUsed = false;
		//
		// 	string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email-verification-code-template.html");
		// 	string htmlFile = await File.ReadAllTextAsync(templatePath);
		// 	string htmlBody = htmlFile.Replace("{{code}}", randomNumberInRange);
		//
		// 	var mail = new EmailReq(To: [user.Email], Subject: "Código de Verificación", Body: htmlBody);
		// 	await SendEmailAsync(mail, configuration);
		// }

		public static string RemoveDiacritics(string text)
		{
			text = text.Trim().ToLower();
			if (string.IsNullOrEmpty(text)) return text;

			// Normaliza el texto a la forma de descomposición (FormD)
			var normalizedText = text.Normalize(NormalizationForm.FormD);

			// Filtra los caracteres no espaciales (diacríticos)
			var sb = new StringBuilder();

			foreach (var c in normalizedText)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					sb.Append(c);
				}
			}

			// Normaliza de nuevo a FormC (composición)
			return sb.ToString().Normalize(NormalizationForm.FormC);
		}

		public async static Task SendEmailAsync(EmailReq request, IConfiguration configuration)
		{
			string host = configuration["Smtp:Host"]!;
			int port = int.Parse(configuration["Smtp:Port"]!);
			string user = configuration["Smtp:AppPassword:User"]!;
			string password = configuration["Smtp:AppPassword:Pass"]!;

			InternetAddressList emailsList = [];
			foreach (var item in request.To) emailsList.Add(MailboxAddress.Parse(item));

			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse(user));
			email.To.AddRange(emailsList);
			email.Subject = request.Subject;
			email.Body = new TextPart(TextFormat.Html) { Text = $"{request.Body}" };

			var smtp = new SmtpClient();
			await smtp.ConnectAsync(host, port);
			await smtp.AuthenticateAsync(user, password);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);
		}
	}
}

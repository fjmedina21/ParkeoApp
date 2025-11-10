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

		public static string GenerateSessionJwtAsync(User entity, IConfiguration configuration)
		{
			byte[] key = Encoding.UTF8.GetBytes(configuration["JWTSettings:key"]!);
			var validHours = int.Parse(configuration["JWTSettings:expiresIn-hours"]!);

			List<Claim> authClaims =
			[
				new("userId", entity.UserId.ToString()),
				new("tenantId", entity.TenantId.ToString()),
			];

			var secretKey = new SymmetricSecurityKey(key);
			var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
			var token = new JwtSecurityToken(claims: authClaims, expires: DateTime.Now.AddHours(validHours), signingCredentials: credentials);
			var handledToken = new JwtSecurityTokenHandler().WriteToken(token);

			return handledToken;
		}

		public static string GenerateRefreshJwtAsync(User entity, IConfiguration configuration)
		{
			byte[] key = Encoding.UTF8.GetBytes(configuration["JWTSettings:key"]!);
			var validHours = int.Parse(configuration["JWTSettings:expiresIn-refreshToken-hours"]!);

			List<Claim> authClaims =
			[
				new("userId", entity.UserId.ToString())
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
			byte[] key = Encoding.UTF8.GetBytes(configuration["JWTSetting:resetPasswordKey"]!);
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

			var exp = int.Parse(jwt.Claims.First(c => c.Type == "exp").Value);
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

		public static void SendReservationEmailNotification(Reservation reservation, string action, string subject, IConfiguration configuration)
		{
			var details = $$"""
			               <b>Código:</b> {{reservation.Code}}<br/>
			               <b>Fecha:</b> {{reservation.StartAt.ToLongDateString()}}<br/>
			               <b>Hora Inicio:</b> {{reservation.StartAt:hh:mm tt}}<br/>
			               <b>Hora Fin:</b> {{reservation.EndAt:hh:mm tt}}<br/>
			               <b>Costo:</b> {{reservation.TotalCost}}<br/>
			               <b>Tipo de espacio:</b> {{reservation.Spot.SpotType}}<br/>
			               <b>Piso:</b> {{reservation.Spot.Floor}}<br/>
			               <b>Parqueo:</b> {{reservation.Spot.ParkingLot.Name}}<br/>
			               <b>Dirección:</b> {{reservation.Spot.ParkingLot.Address}}<br/>
			               <b>Descripción:</b> {{reservation.Spot.ParkingLot.Description}}
			               """;

			var now = DateTime.Now;
			string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reservation-email-template.html");
			string htmlFile = File.ReadAllText(templatePath);
			string htmlBody = htmlFile
				.Replace("{{subject}}", subject)
				.Replace("{{user}}", $"{reservation.User.FirstName} {reservation.User.LastName}")
				.Replace("{{action}}", action)
				.Replace("{{details}}", details)
				.Replace("{{date}}", $"{now.ToLongDateString()} {now.ToLongTimeString()}");

			var mail = new EmailReq(To: [reservation.User.Email], Subject: subject, Body: htmlBody);
			SendEmail(mail, configuration);
		}

		// public async static Task SendVerificationCode( User user ,IConfiguration configuration)
		// {
		// 	Random rnd = new();
		// 	var randomNumberInRange = rnd.Next(100000, 999999).ToString();
		//
		// 	user.VerificationCode = HashText(randomNumberInRange);
		// 	user.VerificationCodeValidUntil = DateTime.UtcNow.AddMinutes(10);
		// 	user.VerificationCodeUsed = false;
		//
		// 	string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "send-code-by-email-template.html");
		// 	string htmlFile = await File.ReadAllTextAsync(templatePath);
		// const string subject = "Código de Verificación";
		// string htmlBody = htmlFile
		// 	.Replace("{{subject}}", subject)
		// 	.Replace("{{code}}", randomNumberInRange);
		//
		// 	var mail = new EmailReq(To: [user.Email], Subject: subject, Body: htmlBody);
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

		public static void SendEmail(EmailReq request, IConfiguration configuration)
		{
			string host = configuration["Smtp:Host"]!;
			var port = int.Parse(configuration["Smtp:Port"]!);
			string user = configuration["Smtp:AppPassword:User"]!;
			string password = configuration["Smtp:AppPassword:Pass"]!;

			InternetAddressList emailsList = [];
			foreach (string item in request.To) emailsList.Add(MailboxAddress.Parse(item));

			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse(user));
			email.To.AddRange(emailsList);
			email.Subject = request.Subject;
			email.Body = new TextPart(TextFormat.Html) { Text = $"{request.Body}" };

			var smtp = new SmtpClient();
			smtp.Connect(host, port);
			smtp.Authenticate(user, password);
			smtp.Send(email);
			smtp.Disconnect(true);
		}
	}
}

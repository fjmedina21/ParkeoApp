namespace ParkeoApp.Application.Services.AuthService.DTO
{
    public record CredentialsDto(string Email, string Password);
    public record ChangePasswordDto(string OldPassword, string NewPassword);
    public record ForgotPasswordDto(string Email);
    public record VerifyEmailDto(string Email, string VerificationCode);

}

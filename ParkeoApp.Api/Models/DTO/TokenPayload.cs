namespace ParkeoApp.Api.Models.DTO
{
    public record TokenPayload(Guid User, Guid Tenant, DateTime ExpiresIn);
    public record RefreshTokenPayload(Guid User,DateTime ExpiresIn);
}

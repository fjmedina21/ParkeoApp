namespace ParkeoApp.Domain.Entities;

public partial class UsersToken
{
    public Guid TokenId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public string AccessToken { get; set; } = null!;

    public DateTime AccessExpiresAt { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime RefreshExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

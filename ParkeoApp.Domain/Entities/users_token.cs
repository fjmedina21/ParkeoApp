namespace ParkeoApp.Domain.Entities;

public partial class users_token
{
    public Guid token_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid user_id { get; set; }

    public string access_token { get; set; } = null!;

    public DateTime access_expires_at { get; set; }

    public string refresh_token { get; set; } = null!;

    public DateTime refresh_expires_at { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? revoked_at { get; set; }

    public bool is_active { get; set; }

    public virtual tenant tenant { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}

namespace ParkeoApp.Domain.Entities;

public partial class user
{
    public Guid user_id { get; set; }

    public Guid tenant_id { get; set; }

    public string firts_name { get; set; } = null!;

    public string last_name { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public string? profile_picture_url { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public virtual ICollection<reservation> reservations { get; set; } = new List<reservation>();

    public virtual tenant tenant { get; set; } = null!;

    public virtual ICollection<user_role> user_roles { get; set; } = new List<user_role>();

    public virtual ICollection<users_token> users_tokens { get; set; } = new List<users_token>();
}

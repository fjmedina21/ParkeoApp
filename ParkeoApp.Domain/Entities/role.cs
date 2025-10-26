namespace ParkeoApp.Domain.Entities;

public partial class role
{
    public Guid role_id { get; set; }

    public Guid tenant_id { get; set; }

    public string name { get; set; } = null!;

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public virtual tenant tenant { get; set; } = null!;

    public virtual ICollection<user_role> user_roles { get; set; } = new List<user_role>();
}

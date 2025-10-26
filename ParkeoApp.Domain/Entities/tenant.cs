namespace ParkeoApp.Domain.Entities;

public partial class tenant
{
    public Guid tenant_id { get; set; }

    public string name { get; set; } = null!;

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public string domain { get; set; } = null!;

    public virtual ICollection<parking_lot> parking_lots { get; set; } = new List<parking_lot>();

    public virtual ICollection<parking_spot> parking_spots { get; set; } = new List<parking_spot>();

    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    public virtual ICollection<reservation> reservations { get; set; } = new List<reservation>();

    public virtual ICollection<role> roles { get; set; } = new List<role>();

    public virtual ICollection<user> users { get; set; } = new List<user>();

    public virtual ICollection<users_token> users_tokens { get; set; } = new List<users_token>();
}

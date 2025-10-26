namespace ParkeoApp.Domain.Entities;

public partial class reservation
{
    public Guid reservation_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid user_id { get; set; }

    public Guid? spot_id { get; set; }

    public string code { get; set; } = null!;

    public DateTime start_at { get; set; }

    public DateTime end_at { get; set; }

    public string status { get; set; } = null!;

    public string? note { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    public virtual parking_spot? spot { get; set; }

    public virtual tenant tenant { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}

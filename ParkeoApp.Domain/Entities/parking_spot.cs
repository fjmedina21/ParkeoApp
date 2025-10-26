namespace ParkeoApp.Domain.Entities;

public partial class parking_spot
{
    public Guid spot_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid parking_lot_id { get; set; }

    public string code { get; set; } = null!;

    public string? spottype { get; set; }

    public string status { get; set; } = null!;

    public int? floor { get; set; }

    public string? note { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public virtual parking_lot parking_lot { get; set; } = null!;

    public virtual ICollection<reservation> reservations { get; set; } = new List<reservation>();

    public virtual tenant tenant { get; set; } = null!;
}

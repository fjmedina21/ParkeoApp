namespace ParkeoApp.Domain.Entities;

public partial class parking_lot
{
    public Guid parking_lot_id { get; set; }

    public Guid tenant_id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public string? address { get; set; }

    public decimal? latitude { get; set; }

    public decimal? longitude { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public virtual ICollection<parking_spot> parking_spots { get; set; } = new List<parking_spot>();

    public virtual tenant tenant { get; set; } = null!;
}

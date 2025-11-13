namespace ParkeoApp.Domain.Entities;

public partial class ParkingLot
{
    public Guid ParkingLotId { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Address { get; set; } = null!;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public decimal HourlyRate { get; set; }

    public int FloorLevels { get; set; }

    public virtual ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();

    public virtual Tenant Tenant { get; set; } = null!;
}

namespace ParkeoApp.Domain.Entities;

public partial class ParkingSpot
{
    public Guid SpotId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ParkingLotId { get; set; }

    public string Code { get; set; } = null!;

    public string SpotType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int Floor { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Tenant Tenant { get; set; } = null!;
}

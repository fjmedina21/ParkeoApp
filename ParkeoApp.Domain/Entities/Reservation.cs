namespace ParkeoApp.Domain.Entities;

public partial class Reservation
{
    public Guid ReservationId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public Guid ParkingSpotId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public decimal TotalCost { get; set; }

    public virtual ParkingSpot ParkingSpot { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

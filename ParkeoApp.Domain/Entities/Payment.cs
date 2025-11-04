namespace ParkeoApp.Domain.Entities;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ReservationId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}

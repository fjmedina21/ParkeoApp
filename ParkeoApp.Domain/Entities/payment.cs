namespace ParkeoApp.Domain.Entities;

public partial class payment
{
    public Guid payment_id { get; set; }

    public Guid tenant_id { get; set; }

    public Guid? reservation_id { get; set; }

    public decimal amount { get; set; }

    public string currency { get; set; } = null!;

    public string? payment_method { get; set; }

    public string status { get; set; } = null!;

    public string? transaction_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual reservation? reservation { get; set; }

    public virtual tenant tenant { get; set; } = null!;
}

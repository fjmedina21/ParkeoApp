namespace ParkeoApp.Domain.Entities;

public partial class Tenant
{
    public Guid TenantId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string Domain { get; set; } = null!;

    public virtual ICollection<ParkingLot> ParkingLots { get; set; } = new List<ParkingLot>();

    public virtual ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<UsersToken> UsersTokens { get; set; } = new List<UsersToken>();
}

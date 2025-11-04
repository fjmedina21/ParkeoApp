namespace ParkeoApp.Domain.Entities;

public partial class Role
{
    public Guid RoleId { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<RolesPermission> RolesPermissions { get; set; } = new List<RolesPermission>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

namespace ParkeoApp.Domain.Entities;

public partial class RolesPermission
{
    public Guid RolePermissionId { get; set; }

    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

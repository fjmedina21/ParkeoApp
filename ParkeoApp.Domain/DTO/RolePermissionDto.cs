namespace ParkeoApp.Domain.DTO
{
	public class GetRolePermission
	{
		public Guid RolePermissionId { get; set; }
		public DateTime AssignedAt { get; set; }
		public virtual GetPermission Permission { get; set; } = null!;
		public virtual GetRole Role { get; set; } = null!;
	}

	public class GetRolePermissionWnRole
	{
		public Guid RolePermissionId { get; set; }
		public DateTime AssignedAt { get; set; }
		public virtual GetPermission Permission { get; set; } = null!;
	}

	public class GetRolePermissionWnPermission
	{
		public Guid RolePermissionId { get; set; }
		public DateTime AssignedAt { get; set; }
		public virtual GetRole Role { get; set; } = null!;
	}

	public record AddRolePermission(Guid RoleId, Guid PermissionId);
}

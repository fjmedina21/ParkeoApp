namespace ParkeoApp.Domain.DTO
{
	public class GetRole
	{
		public Guid RoleId { get; init; }
		public string Name { get; init; } = null!;
		// public virtual ICollection<GetRolePermissionWNRole> RolesPermissions { get; set; } = [];
	}

	public record AddRole(string Name /*, ICollection<GetPermission> RolesPermissions*/);
}

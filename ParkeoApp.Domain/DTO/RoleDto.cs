namespace ParkeoApp.Domain.DTO
{
	public class GetRole
	{
		public Guid RoleId { get; set; }
		public string Name { get; set; } = null!;
		// public virtual ICollection<GetRolePermissionWNRole> RolesPermissions { get; set; } = [];
	}

	public record AddRole(string Name /*, ICollection<GetPermission> RolesPermissions*/);
}

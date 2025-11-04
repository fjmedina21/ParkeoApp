namespace ParkeoApp.Domain.DTO
{
	public class GetUserRole
	{
		public Guid UserRoleId { get; set; }
		public DateTime AssignedAt { get; set; }
		public virtual GetPermission Permission { get; set; } = null!;
		public virtual GetRole Role { get; set; } = null!;
	}

	public class GetUserRoleWNRole
	{
		public Guid UserRoleId { get; set; }
		public DateTime AssignedAt { get; set; }
		public virtual GetUser User { get; set; } = null!;
	}

	public class GetUserRoleWNUser
	{
		public Guid UserRoleId { get; set; }
		public DateTime AssignedAt { get; set; }
		public virtual GetRole Role { get; set; } = null!;
	}

	public record AddUserRole(Guid RoleId, Guid UserId);
}

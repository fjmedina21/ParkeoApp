namespace ParkeoApp.Domain.DTO
{
	public class GetPermission
	{
		public Guid PermissionId { get; set; }
		public string Name { get; set; } = null!;
	}

	public record AddPermission(string Name);
}

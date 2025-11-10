namespace ParkeoApp.Domain.DTO
{
	public class GetPermission
	{
		public Guid PermissionId { get; init; }
		public string Name { get; init; } = null!;
	}

	public record AddPermission(string Name);
}

namespace ParkeoApp.Domain.DTO
{
	public class GetTenant
	{
		public Guid TenantId { get; init; }
		public string Name { get; init; } = null!;
		public string Domain { get; init; } = null!;
	}

	public record AddTenant(string Name, string Domain);
}

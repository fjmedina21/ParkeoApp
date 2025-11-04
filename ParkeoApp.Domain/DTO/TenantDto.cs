namespace ParkeoApp.Domain.DTO
{
	public class GetTenant
	{
		public Guid TenantId { get; set; }
		public string Name { get; set; } = null!;
		public string Domain { get; set; } = null!;
	}

	public record AddTenant(string Name, string Domain);
}

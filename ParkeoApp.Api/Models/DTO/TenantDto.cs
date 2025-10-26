namespace ParkeoApp.Api.Models.DTO
{
	public class GetTenant
	{
		public Guid tenant_id { get; set; }
		public string name { get; set; } = null!;
		public string domain { get; set; } = null!;
	}

	public record AddTenant(string name, string domain);
}

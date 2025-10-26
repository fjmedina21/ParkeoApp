using ParkeoApp.Api.Models.Entities;

namespace ParkeoApp.Api.Models.DTO
{
	public class GetUser
	{
		public Guid user_id { get; set; }
		public string firts_name { get; set; }
		public string last_name { get; set; }
		public string email { get; set; } = null!;
		public string? profile_picture_url { get; set; }
		public virtual ICollection<reservation> reservations { get; set; } = new List<reservation>();
	}

	public record AddUser(Guid tenant_id,string firts_name, string last_name, string email, string password);
}

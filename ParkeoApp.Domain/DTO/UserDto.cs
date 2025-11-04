using ParkeoApp.Domain.Entities;

namespace ParkeoApp.Domain.DTO
{
	public class GetUser
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; } = null!;
		public string? ProfilePictureUrl { get; set; }
	}

	public class GetUserWNRef
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; } = null!;
		public string? ProfilePictureUrl { get; set; }
	}

	public record AddUser(Guid TenantId, string FirstName, string LastName, string Email, string Password);
}

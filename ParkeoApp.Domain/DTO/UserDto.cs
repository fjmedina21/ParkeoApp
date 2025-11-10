using ParkeoApp.Domain.Entities;

namespace ParkeoApp.Domain.DTO
{
	public class GetUser
	{
		public Guid UserId { get; init; }
		public string FirstName { get; init; } = null!;
		public string LastName { get; init; } = null!;
		public string Email { get; init; } = null!;
		public string? ProfilePictureUrl { get; init; }
	}

	public class GetUserWnRef
	{
		public Guid UserId { get; init; }
		public string FirstName { get; init; }= null!;
		public string LastName { get; init; }= null!;
		public string Email { get; init; } = null!;
		public string? ProfilePictureUrl { get; init; }
	}

	public record AddUser(Guid TenantId, string FirstName, string LastName, string Email, string Password);
}

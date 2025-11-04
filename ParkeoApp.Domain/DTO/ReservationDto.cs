using ParkeoApp.Domain.Entities;

namespace ParkeoApp.Domain.DTO
{
	public class GetReservation
	{
		public Guid ReservationId { get; set; }
		public string Code { get; set; } = null!;
		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
		public string Status { get; set; } = null!;
		// public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
		public virtual GetParkingSpot Spot { get; set; } = null!;
		public virtual GetUserWNRef User { get; set; } = null!;
	}

	public class GetReservationWNRef
	{
		public Guid ReservationId { get; set; }
		public string Code { get; set; } = null!;
		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
		public string Status { get; set; } = null!;
		// public virtual ICollection<Payment> Payments { get; set; } = [];
		public virtual GetParkingSpot Spot { get; set; } = null!;
	}

	public record AddReservation(
		DateTime StartAt,
		DateTime EndAt
	);
}

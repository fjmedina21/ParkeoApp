using ParkeoApp.Domain.Entities;

namespace ParkeoApp.Domain.DTO
{
	public class GetReservation
	{
		public Guid ReservationId { get; init; }
		public string Code { get; init; } = null!;
		public decimal TotalCost { get; set; }
		public DateTime StartAt { get; init; }
		public DateTime EndAt { get; init; }
		public string Status { get; init; } = null!;
		// public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
		public virtual GetParkingSpot Spot { get; init; } = null!;
	}

	public record AddReservation(
		Guid SpotId,
		DateTime StartAt,
		DateTime EndAt
	);
}

namespace ParkeoApp.Domain.DTO
{
	public class GetParkingLot
	{
		public Guid ParkingLotId { get; init; }
		public string Name { get; init; } = null!;
		public string? Description { get; init; }
		public string Address { get; init; } = null!;
		public decimal HourlyRate { get; init; }
		public int FloorLevels { get; set; }
		public double Latitude { get; init; }
		public double Longitude { get; init; }
		public double? DistanceKm { get; set; }
		public int Available { get; init; }
		public int Occupied { get; init; }
		public int Maintenance { get; init; }
	}

	public record AddParkingLot(
		string Name,
		string? Description,
		decimal HourlyRate,
		int FloorLevels,
		string Address,
		double Latitude,
		double Longitude,
		ICollection<AddParkingSpot> ParkingSpots
		);
}

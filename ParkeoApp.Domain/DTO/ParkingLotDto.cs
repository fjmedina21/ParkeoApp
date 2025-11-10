namespace ParkeoApp.Domain.DTO
{
	public class GetParkingLot
	{
		public Guid ParkingLotId { get; init; }
		public string Name { get; init; } = null!;
		public string? Description { get; init; }
		public string Address { get; init; } = null!;
		public decimal HourlyRate { get; init; }
		public decimal Latitude { get; init; }
		public decimal Longitude { get; init; }
		public int Available { get; init; }
		public int Occupied { get; init; }
		public int Reserved { get; init; }
	}

	public class GetParkingLotWnRef
	{
		public Guid ParkingLotId { get; init; }
		public string Name { get; init; } = null!;
		public string? Description { get; init; }
		public string Address { get; init; } = null!;
		public decimal HourlyRate { get; set; }
		public decimal Latitude { get; init; }
		public decimal Longitude { get; init; }
	}

	public record AddParkingLot(
		string Name,
		string? Description,
		decimal HourlyRate,
		string Address,
		decimal Latitude,
		decimal Longitude,
		ICollection<AddParkingSpot> ParkingSpots
		);
}

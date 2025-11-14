namespace ParkeoApp.Domain.DTO
{
	public class GetParkingSpot
	{
		public Guid ParkingSpotId { get; init; }
		public string Code { get; init; } = null!;
		public string SpotType { get; init; } = null!;
		public string Status { get; init; } = null!;
		public int Floor { get; init; }
		public GetParkingLot ParkingLot { get; init; } = null!;
	}

	public class GetParkingSpotWnRef
	{
		public Guid ParkingSpotId { get; init; }
		public string Code { get; init; } = null!;
		public string SpotType { get; init; } = null!;
		public string Status { get; init; } = null!;
		public int Floor { get; init; }
	}

	public record AddParkingSpot(
		Guid? ParkingLotId,
		string Code,
		string SpotType,
		string Status,
		int Floor
	);
}

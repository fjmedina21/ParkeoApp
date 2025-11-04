namespace ParkeoApp.Domain.DTO
{
	public class GetParkingSpot
	{
		public Guid SpotId { get; set; }
		public string Code { get; set; } = null!;
		public string SpotType { get; set; }
		public string Status { get; set; } = null!;
		public int Floor { get; set; }
		public virtual GetParkingLotWNRef ParkingLot { get; set; } = null!;
	}

	public class GetParkingSpotWNRef
	{
		public Guid SpotId { get; set; }
		public string Code { get; set; } = null!;
		public string SpotType { get; set; }
		public string Status { get; set; } = null!;
		public int Floor { get; set; }
	}

	public record AddParkingSpot(
		Guid? ParkingLotId,
		string Code,
		string SpotType,
		string Status,
		int Floor
	);
}

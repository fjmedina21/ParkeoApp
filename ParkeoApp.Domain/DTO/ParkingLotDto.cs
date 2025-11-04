namespace ParkeoApp.Domain.DTO
{
	public class GetParkingLot
	{
		public Guid ParkingLotId { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public string Address { get; set; } = null!;
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
		public int Quantity { get; set; }
		public virtual ICollection<GetParkingSpotWNRef> ParkingSpots { get; set; } = [];
	}

	public class GetParkingLotWNRef
	{
		public Guid ParkingLotId { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public string Address { get; set; } = null!;
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
		public int Quantity { get; set; }
	}

	public record AddParkingLot(
		string Name,
		string? Description,
		int Quantity,
		string Address,
		decimal Latitude,
		decimal Longitude,
		ICollection<AddParkingSpot> ParkingSpots
		);
}

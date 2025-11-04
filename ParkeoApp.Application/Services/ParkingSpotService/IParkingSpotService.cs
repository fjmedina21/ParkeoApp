using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Services.ParkingSpotService
{
	public interface IParkingSpotService : IBaseService<GetParkingSpot, AddParkingSpot>
	{
		public Task<ApiResponse<GetReservationWNRef>> CreateReservationAsync(Guid uid, AddReservation reservation, string jwt);
	}
}

using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Services.ParkingSpotService
{
	public interface IParkingSpotService : IBaseService<GetParkingSpot, AddParkingSpot>
	{
		Task<ApiResponse<GetParkingSpot>> GetByParkingLotAsync(Guid parkingLotId, PaginationParams paginationParams, string jwt);
	}
}

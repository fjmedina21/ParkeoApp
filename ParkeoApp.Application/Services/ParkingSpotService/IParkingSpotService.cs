using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Services.ParkingSpotService
{
	public interface IParkingSpotService : IBaseService<GetParkingSpotWnRef, AddParkingSpot>
	{
		Task<ApiResponse<GetParkingSpotWnRef>> GetByParkingLotAsync(Guid parkingLotId, PaginationParams paginationParams, string jwt);
		Task<ApiResponse> MarkSpotAsMaintenanceAsync(Guid uid, string jwt);
		Task<ApiResponse> MarkSpotAsAvailableAsync(Guid uid, string jwt);
	}
}
